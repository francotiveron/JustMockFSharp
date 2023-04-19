type A() =
    let agent = MailboxProcessor.Start(fun mbx ->
        let mutable field: int = 0
        
        let f1() = 5

        let rec loop () = async {
            try 
                match! mbx.Receive() with
                | _ ->
                    let f2() = 6
                    ()
            with | _ -> ()
            
            return! loop() }

        loop ())

let a = typeof<A>
