module InfluxTests

open Xunit
open System
open Telerik.JustMock
open Telerik.JustMock.Helpers
open InfluxDB.Client
open InfluxDB.Client.Core.Flux.Domain
open System.Threading.Tasks
open System.Threading


[<Fact>]
let ``mock third party API call deep inside user code``() =
    // ?? Mock 'qApi.QueryAsync' in JustMockFSharp.Influx line 16
    
    let app = new Application.Application()
    app.Method1()

[<Fact>]
let ``tentative 1``() =
    let queryAsyncMock _ _ _ (onNext: FluxRecord -> unit) _ _ : Task = task {
        let stop = onNext null
        ()
    }

    let queryApiMock = Mock.Create<QueryApi>(Behavior.Strict)

    //Mock
    //    //.Arrange<Task>(fun() -> queryApiMock.QueryAsync(Arg.IsAny<string>(), Arg.IsAny<FluxRecord -> unit>(), Arg.IsAny<exn -> unit>(), Arg.IsAny<unit -> unit>(), Arg.IsAny<string>(), Arg.IsAny<CancellationToken>()))
    //    .Arrange<Task>(fun() -> queryApiMock.QueryAsync(Arg.IsAny<string>(), Arg.IsAny<Action<FluxRecord>>(), Arg.IsAny<Action<exn>>(), Arg.IsAny<Action>(), Arg.IsAny<string>(), Arg.IsAny<CancellationToken>()))
    //    .Returns(Func<_, _, _, _, _, _, Task>(queryAsyncMock))
    //    .OnAllThreads() |> ignore

    queryApiMock
        .Arrange(fun mock -> mock.QueryAsync(Arg.IsAny<string>(), Arg.IsAny<Action<FluxRecord>>(), Arg.IsAny<Action<exn>>(), Arg.IsAny<Action>(), Arg.IsAny<string>(), Arg.IsAny<CancellationToken>()))
        //.IgnoreInstance()
        .Returns(Func<_, _, _, _, _, _, Task>(queryAsyncMock))
        .OnAllThreads()
        |> ignore

    let clientMock = Mock.Create<InfluxDBClient>(Constructor.Mocked, Behavior.Strict)
    clientMock.Arrange(fun mock -> mock.GetQueryApi()).Returns(queryApiMock).OnAllThreads() |> ignore

    Mock.Arrange<_>(fun() -> InfluxDBClientOptions.Builder().Build()).DoNothing().OnAllThreads() |> ignore
    Mock.Arrange<_>(fun() -> new InfluxDBClient(Arg.IsAny<InfluxDBClientOptions>())).Returns(clientMock).OnAllThreads() |> ignore

    let app = new Application.Application()
    app.Method1().Wait()
