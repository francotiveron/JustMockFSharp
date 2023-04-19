namespace N1

open System
type T1() = class end

[<AutoOpen>]
module Extensions = 
    type T1 with
        member __.ExtendedProperty = DateTime.MinValue

namespace N2
open N1

type Service = 
    static member GetItems(): T1 array = [||]

namespace N3
open N1
open N2
open System

type private Msg = | Callback of (DateTime -> unit)

type T2() = 
    let agent = MailboxProcessor.Start(fun mbx ->
        let mutable items = Service.GetItems()
        let rec loop () = async {
            match! mbx.Receive() with
            | Callback callback -> 
                items <- Service.GetItems()
                let ep = items[0].ExtendedProperty
                callback ep
            return! loop() }

        loop ())
    
    member __.Do(callback) = agent.Post(Callback callback)
