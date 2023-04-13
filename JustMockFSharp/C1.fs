namespace C1

open System.Threading.Tasks
open System.Threading

type C1() = 
    let mutable field1 = 0

    let parameterLess() = field1 <- 21

    let parameterized (ct: CancellationToken) (callback: Task -> unit) = 
        if ct.IsCancellationRequested then callback Task.CompletedTask

    let parameterized1 (ct: CancellationToken) = ()
    
    let parameterized2 (callback: Task -> unit) = ()

    let f1 (callback: Task -> unit) = ()
    let f2 (callback: Task -> unit) = ()

