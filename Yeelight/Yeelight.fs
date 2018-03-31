module Yeelight

    open System.IO
    open System.Net
    open System.Net.Sockets
    open System.Text
    open System.Text.RegularExpressions

    type Power = Off | On
    type Brightness = int
    type Temperature = int
    type Red = int
    type Green = int
    type Blue = int
    type Hue = int
    type Saturation = int
    type Color = Rgb of Red * Green * Blue | Hsv of Hue * Saturation
    type Effect = Sudden | Smooth of uint32
    type Name = string
    type Response = Ok | Error of string

    type private Method =
        | SetPower
        | Toggle
        | SetBrightness
        | SetTemperature
        | SetRgb
        | SetHsv
        | SetName

    type private Parameter =
        | Power of Power
        | Brightness of Brightness
        | Temperature of Temperature
        | Color of Color
        | Effect of Effect
        | Name of Name

    let private port = 55443

    // Attempt to match the specified pattern using regular expressions and return a list of (string) capture groups
    let private (|Regex|_|) pattern input =
        let m = Regex.Match (input, pattern)
        match m.Success with
        | true ->
            m.Groups
                |> Seq.cast<Group>
                |> List.ofSeq
                |> List.map string
                |> Some
        | false -> None

    let private send (stream : Stream) (data : string) =
        async {
            let buffer = Encoding.UTF8.GetBytes data
            do! stream.AsyncWrite buffer
        }

    let private receive (stream : Stream) =
        async {
            let buffer = Array.zeroCreate<byte> 1024
            do! stream.AsyncRead buffer |> Async.Ignore
            use memoryStream = new MemoryStream ()
            do! memoryStream.AsyncWrite buffer
            return memoryStream.ToArray () |> Encoding.UTF8.GetString
        }

    let private parseResponse = function
        // Successful requests all return this response
        | Regex "\"result\":\[\"ok\"\]" _ ->
            Ok
        // Unsuccessful requests return a response containing an integer code and a string message
        | Regex "\"error\":{\"code\":(-?\\d+),\"message\":\"(.*)\"}" matches ->
            Error (List.last matches)
        | _ ->
            Error "invalid response"

    let private stringifyMethod = function
        | SetPower -> "set_power"
        | Toggle -> "toggle"
        | SetBrightness -> "set_bright"
        | SetTemperature -> "set_ct_abx"
        | SetRgb -> "set_rgb"
        | SetHsv -> "set_hsv"
        | SetName -> "set_name"

    // string parameters should always be quoted; int parameters should not
    let private stringifyParameter = function
        | Power power ->
            match power with
            | Off -> "\"off\""
            | On -> "\"on\""
        | Brightness brightness ->
            string brightness
        | Temperature temperature ->
            string temperature
        | Color color ->
            match color with
            // RGB is represented as an integer 0 to 16777215 (0xFFFFFF) where each component is 2 bytes
            | Rgb (r, g, b) ->
                let r = uint32 r &&& 0xFFu <<< 16
                let g = uint32 g &&& 0xFFu <<< 8
                let b = uint32 b &&& 0xFFu
                let rgb = r ||| g ||| b
                string rgb
            // HSV is represented as an integer 0 to 359 for hue and an integer 0 to 100 for saturation
            | Hsv (h, s) ->
                sprintf "%d,%d" h s
        | Effect effect ->
            // Effect consists of "sudden"|"smooth" plus a uint32 duration.
            match effect with
            | Sudden ->
                // Duration is ignored by the device but must be included in the request; set to 0
                "\"sudden\",0"
            | Smooth duration ->
                sprintf "\"smooth\",%d" duration
        | Name name ->
            sprintf "\"%s\"" name

    let private communicate method parameters (address : IPAddress) =
        let method = stringifyMethod method
        let parameters =
            parameters
                |> List.map stringifyParameter
                |> String.concat ","
        // Requests must end with CRLF
        let request = sprintf "{\"id\":0,\"method\":\"%s\",\"params\":[%s]}\r\n" method parameters
        async {
            use client = new TcpClient ()
            do! client.ConnectAsync (address, port) |> Async.AwaitTask
            use stream = client.GetStream ()
            do! send stream request
            let! response = receive stream
            return parseResponse response
        }

    let setPower power effect =
        communicate SetPower [Power power; Effect effect]

    let toggle : IPAddress -> Async<Response> =
        communicate Toggle []

    let off : Effect -> IPAddress -> Async<Response> =
        setPower Off

    let on : Effect -> IPAddress -> Async<Response> =
        setPower On

    let setBrightness brightness effect =
        communicate SetBrightness [Brightness brightness; Effect effect]

    let setTemperature temperature effect =
        communicate SetTemperature [Temperature temperature; Effect effect]

    let setColor color effect =
        let method =
            match color with
            | Rgb _ -> SetRgb
            | Hsv _ -> SetHsv
        communicate method [Color color; Effect effect]

    let setName name =
        communicate SetName [Name name]
