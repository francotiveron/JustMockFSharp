module Tests

open System
open System.Reflection
open Xunit
open Telerik.JustMock
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Core

//Ok
[<Fact>]
let ``mock internal module functions``() =
    Mock.Arrange<int>(fun _ -> M1.M2.f1()).Returns(5) |> ignore

    let ret = M1.M2.f2()

    Assert.Equal(10, ret)

//Ok
[<Fact>]
let ``call parameterless private function in class`` () =
    let ca = Mock.Create<C1.C1>()
    Mock.NonPublic.Arrange(ca, "parameterLess").CallOriginal() |> ignore
    let inst = PrivateAccessor(ca)
    inst.CallMethod("parameterLess") |> ignore
    let field1 = unbox <| inst.GetField("field1")
    Assert.Equal(21, field1)

//NulRef in line 36
[<Fact>]
let ``1 - call parameterized private function in class``() =
    let ca = Mock.Create<C1.C1>()
    let argTypes = [|typeof<CancellationToken>; typeof<Task -> unit>|]
    let args = [|box CancellationToken.None; box (fun (_: Task) -> ())|]
    Mock.NonPublic.Arrange(ca, "parameterized", argTypes, args).CallOriginal() |> ignore
    let inst = new PrivateAccessor(ca)
    inst.CallMethod("parameterized") |> ignore
    Assert.True(true)

//Changed Type for second argument, no difference
[<Fact>]
let ``2 - call parameterized private function in class``() =
    let ca = Mock.Create<C1.C1>()
    let argTypes = [| typeof<CancellationToken>; typeof<FSharpFunc<Task, unit>> |]
    let args = [|box CancellationToken.None; box (fun (_: Task) -> ())|]
    Mock.NonPublic.Arrange(ca, "parameterized", argTypes, args).CallOriginal() |> ignore
    let inst = new PrivateAccessor(ca)
    inst.CallMethod("parameterized") |> ignore
    Assert.True(true)

//Moved argTypes and args later when calling function
//Now it fails with ArgumentException when calling (line 61)
[<Fact>]
let ``3 - call parameterized private function in class``() =
    let ca = Mock.Create<C1.C1>()
    let argTypes = [| typeof<CancellationToken>; typeof<FSharpFunc<Task, unit>> |]
    let args = [|box CancellationToken.None; box (fun (_: Task) -> ())|]
    Mock.NonPublic.Arrange(ca, "f1").CallOriginal() |> ignore
    let inst = new PrivateAccessor(ca)
    inst.CallMethod("f1", argTypes, args) |> ignore
    Assert.True(true)

//The offending argument is the function
[<Fact>]
let ``call f1 using JustMock``() =
    let ca = Mock.Create<C1.C1>()
    let argTypes = [| typeof<FSharpFunc<Task, unit>> |]
    let args = [| box (fun (_: Task) -> ()) |]
    Mock.NonPublic.Arrange(ca, "f1").CallOriginal() |> ignore
    let inst = new PrivateAccessor(ca)
    inst.CallMethod("f1", argTypes, args) |> ignore
    Assert.True(true)

//However calling the same function through reflection directly (not through JustMock) succeeds
[<Fact>]
let ``call f1 using reflection``() =
    let f1Mi = typeof<C1.C1>.GetMethod("f1", BindingFlags.Instance ||| BindingFlags.NonPublic)
    let c1 = C1.C1()
    f1Mi.Invoke(c1, [|box (fun (_: Task) -> ())|]) |> ignore
    Assert.True(true)

