module Tests1

open System.Reflection
open Xunit
open System.Threading.Tasks
open Microsoft.FSharp.Core

type C2() = 
    let f1 (callback: Task -> unit) = ()
    let f2 (callback: Task -> unit) = ()

let f3 (_:Task) = ()

[<Fact>]
let ``typesTest``() =
    let f4 = fun (_:Task) -> ()
    let f5 (_:Task) = ()

    let f1Type = (typeof<C2>.GetMethod("f1", BindingFlags.Instance ||| BindingFlags.NonPublic).GetParameters()[0]).ParameterType
    let f2Type = (typeof<C2>.GetMethod("f2", BindingFlags.Instance ||| BindingFlags.NonPublic).GetParameters()[0]).ParameterType
    let f3Type = f3.GetType()
    let f4Type = f4.GetType()
    let f5Type = f5.GetType()

    Assert.Equal(f1Type, f2Type)
    Assert.NotEqual(f1Type, f3Type)
    Assert.NotEqual(f1Type, f4Type)
    Assert.NotEqual(f1Type, f5Type)
    Assert.Equal(f1Type, f3Type.BaseType)
    Assert.Equal(f1Type, f4Type.BaseType)
    Assert.Equal(f1Type, f5Type.BaseType)
