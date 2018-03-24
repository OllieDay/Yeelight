module YeelightTests

    open FsUnit
    open System.Net
    open Xunit

    let address = IPAddress.Parse "192.168.0.x"

    [<Fact>]
    let ``set power off suddenly`` () =
        address
            |> Yeelight.setPower Yeelight.Off Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let ``set power on suddenly`` () =
        address
            |> Yeelight.setPower Yeelight.On Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let toggle () =
        address
            |> Yeelight.toggle
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let off () =
        address
            |> Yeelight.off Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let on () =
        address
            |> Yeelight.on Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok
