module ExtensionsTests

open System
open Xunit
open Telerik.JustMock
open Telerik.JustMock.Helpers
open Microsoft.FSharp.Core
open N1
open N2
open N3
open System.Threading

[<Fact>]
let ``mock extended property``() =
    let now = DateTime.UtcNow
    //let mock = Mock.CreateLike<T1>(fun inst -> inst.ExtendedProperty = now)
    let mock = Mock.Create<T1>()
    mock.Arrange(fun inst -> inst.ExtendedProperty).Returns(now).OnAllThreads() |> ignore
    Assert.Equal(now, mock.ExtendedProperty)

    Mock
        .Arrange<T1 array>(fun() -> Service.GetItems())
        .Returns([|mock|])
        .OnAllThreads()
        |> ignore

    use mres = new ManualResetEventSlim()
    let mutable _ep = DateTime.MaxValue
    T2().Do(fun ep -> _ep <- ep; mres.Set())

    mres.Wait()
    Assert.Equal(now, _ep)
