module Optional
open System.Threading.Tasks

type Filter = 
    | Raw of filter:string
    | Tag of tags:list<string * string>

type ISeries = 
    abstract member TryLatest_<'TItem> : selector:string * ?filter:Filter -> Task<'TItem option>

type Opt() = 
    interface ISeries with
        member __.TryLatest_(selector: string, ?filter: Filter) = Task.FromResult(None)

type App(series: ISeries) = 
    let agent = MailboxProcessor.Start(fun mbx ->
        let rec loop () = async {
            match! mbx.Receive() with
            | (ch: AsyncReplyChannel<Result<_,_>>) -> 
                try
                    let! option = series.TryLatest_("")
                    ch.Reply(Ok option)
                with | x -> ch.Reply(Error x)
            return! loop() }

        loop ())
    
    member __.Do() = task { return agent.PostAndReply(id) }
