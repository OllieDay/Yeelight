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
    let ``off suddenly`` () =
        address
            |> Yeelight.off Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let ``on suddenly`` () =
        address
            |> Yeelight.on Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 100>]
    let ``set brightness suddenly`` brightness =
        address
            |> Yeelight.setBrightness brightness Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Theory>]
    [<InlineData 1700>]
    [<InlineData 6500>]
    let ``set temperature suddenly`` temperature =
        address
            |> Yeelight.setTemperature temperature Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let ``set name to "Yeelight"`` () =
        address
            |> Yeelight.setName "Yeelight"
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Theory>]
    [<InlineData (255, 0, 0)>]
    [<InlineData (0, 255, 0)>]
    [<InlineData (0, 0, 255)>]
    let ``set color to RGB suddenly`` rgb =
        address
            |> Yeelight.setColor (Yeelight.Rgb rgb) Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Theory>]
    [<InlineData (0, 50)>]
    [<InlineData (180, 100)>]
    let ``set color to HSV suddenly`` hsv =
        address
            |> Yeelight.setColor (Yeelight.Hsv hsv) Yeelight.Sudden 0
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok
