namespace Library

module internal Influx =

    open InfluxDB.Client

    [<AutoOpen>]
    module private Internals =
        let client =
            let options = InfluxDBClientOptions.Builder().Build()
            let client = new InfluxDBClient(options)
            client

    let stream (org:string) onNext onError onComplete (flux:string) = backgroundTask {
        let qApi = client.GetQueryApi()
        return! qApi.QueryAsync(flux, onNext, onError, onComplete, org) //Mock this
    }

open System.Threading.Tasks

type InfluxSeries() = 
    member __.Stream<'TItem>(onNext, onError, onComplete) = 
        Influx.stream "" onNext onError onComplete "" :> Task

namespace Application
open Library

type Application() = 
    let series = InfluxSeries()

    member __.Method1() = series.Stream<int>(...)
