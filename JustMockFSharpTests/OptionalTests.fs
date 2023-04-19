module OptionalTests

open System
open FSharp.Core
open Xunit
open Telerik.JustMock
open System.Threading.Tasks
open Optional

type Foo = { AnInt: int }

type SeriesMockBuilder internal () =
    let series = Mock.Create<ISeries>(Behavior.Strict)

    member this.TryLatest_<'T>(?t: 'T) = 
        Mock
            .Arrange<Task<'T option>>(fun() -> series.TryLatest_<'T>(Arg.IsAny<string>(), Arg.IsAny<Filter>()))
            .Returns(Func<string, Filter, Task<'T option>>(fun _ _ -> Task.FromResult(t))) |> ignore

        Mock
            .Arrange<Task<'T option>>(fun() -> series.TryLatest_<'T>(Arg.IsAny<string>()))
            .Returns(Func<string, Task<'T option>>(fun _ -> Task.FromResult(t))) |> ignore

        this

    member __.Build() = series

module Series = 
    let mock builder = builder <| SeriesMockBuilder()

[<Fact>]
let ``mock method with optional argument``() = task {
    let series =  Series.mock (fun builder -> builder.TryLatest_<obj>({AnInt = 1}).Build() )
    let app = Mock.Create<App>(Behavior.CallOriginal, box series)

    match! app.Do() with
    | Ok _ -> () 
    | Error x -> Assert.Fail(x.Message) }



