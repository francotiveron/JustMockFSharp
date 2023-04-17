module InfluxTests

open Xunit
open Telerik.JustMock


[<Fact>]
let ``mock third party API call deep inside user code``() =
    // ?? Mock 'qApi.QueryAsync' in JustMockFSharp.Influx line 16
    
    let app = new Application.Application()
    app.Method1()