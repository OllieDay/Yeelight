module YeelightTests

    open FsUnit
    open System.Net
    open Xunit

    let address = IPAddress.Parse "192.168.0.x"

    [<Fact>]
    let ``set power off`` () =
        address
            |> Yeelight.setPower Yeelight.Off Yeelight.Sudden
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let ``set power on`` () =
        address
            |> Yeelight.setPower Yeelight.On Yeelight.Sudden
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let toggle () =
        address
            |> Yeelight.toggle
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let ``off`` () =
        address
            |> Yeelight.off Yeelight.Sudden
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Fact>]
    let ``on`` () =
        address
            |> Yeelight.on Yeelight.Sudden
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 100>]
    let ``set brightness`` brightness =
        address
            |> Yeelight.setBrightness brightness Yeelight.Sudden
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Theory>]
    [<InlineData 1700>]
    [<InlineData 6500>]
    let ``set temperature`` temperature =
        address
            |> Yeelight.setTemperature temperature Yeelight.Sudden
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
    let ``set color to RGB`` rgb =
        address
            |> Yeelight.setColor (Yeelight.Rgb rgb) Yeelight.Sudden
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok

    [<Theory>]
    [<InlineData (0, 50)>]
    [<InlineData (180, 100)>]
    let ``set color to HSV`` hsv =
        address
            |> Yeelight.setColor (Yeelight.Hsv hsv) Yeelight.Sudden
            |> Async.RunSynchronously
            |> should equal Yeelight.Ok
