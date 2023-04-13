module Tests

open System
open System.Reflection
open Xunit
open Telerik.JustMock
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Core

[<Fact>]
let ``mock internal module functions``() =
    Mock.Arrange<int>(fun _ -> M1.M2.f1()).Returns(5) |> ignore

    let ret = M1.M2.f2()

    Assert.Equal(10, ret)

[<Fact>]
let ``call parameterless private function in class`` () =
    let ca = Mock.Create<C1.C1>()
    Mock.NonPublic.Arrange(ca, "parameterLess").CallOriginal() |> ignore
    let inst = PrivateAccessor(ca)
    inst.CallMethod("parameterLess") |> ignore
    let field1 = unbox <| inst.GetField("field1")
    Assert.Equal(21, field1)

[<Fact>]
let ``mock parameterized private function in class 1``() =
    let ca = Mock.Create<C1.C1>()
    let argTypes = [|typeof<CancellationToken>; typeof<Task -> unit>|]
    let args = [|box CancellationToken.None; box (fun _ -> ())|]
    Mock.NonPublic.Arrange(ca, "parameterized", argTypes, args).CallOriginal() |> ignore
    let inst = new PrivateAccessor(ca)
    inst.CallMethod("parameterized") |> ignore
    Assert.True(true)

[<Fact>]
let ``mock parameterized private function in class 2``() =
    let ca = Mock.Create<C1.C1>()
    let argTypes = [| typeof<CancellationToken>; typeof<FSharpFunc<Task, unit>> |]
    let args = [|box CancellationToken.None; box (fun _ -> ())|]
    Mock.NonPublic.Arrange(ca, "parameterized", argTypes, args).CallOriginal() |> ignore
    let inst = new PrivateAccessor(ca)
    inst.CallMethod("parameterized") |> ignore
    Assert.True(true)
