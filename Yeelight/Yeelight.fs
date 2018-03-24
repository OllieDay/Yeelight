module Yeelight

    open System.IO
    open System.Net
    open System.Net.Sockets
    open System.Text
    open System.Text.RegularExpressions

    type Power = Off | On
    type Brightness = int
    type Effect = Sudden | Smooth
    type Duration = int
    type Response = Ok | Error of string

    type private Method = SetPower | Toggle | SetBrightness
    type private Parameter =
        | Power of Power
        | Brightness of Brightness
        | Effect of Effect
        | Duration of Duration

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

    // string parameters should always be quoted; int parameters should not
    let private stringifyParameter = function
        | Power power ->
            match power with
            | Off -> "\"off\""
            | On -> "\"on\""
        | Brightness brightness ->
            string brightness
        | Effect effect ->
            match effect with
            | Sudden -> "\"sudden\""
            | Smooth -> "\"smooth\""
        | Duration duration ->
            string duration

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

    let setPower power effect duration address =
        communicate SetPower [Power power; Effect effect; Duration duration] address

    let toggle : IPAddress -> Async<Response> =
        communicate Toggle []

    let off : Effect -> Duration -> IPAddress -> Async<Response> =
        setPower Off

    let on : Effect -> Duration -> IPAddress -> Async<Response> =
        setPower On

    let setBrightness brightness effect duration =
        communicate SetBrightness [Brightness brightness; Effect effect; Duration duration]
