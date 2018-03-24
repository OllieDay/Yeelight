# Yeelight
Library for controlling Xiaomi Yeelight Smart LED Bulb.

## Usage

```fsharp
let address = IPAddress.parse "192.168.0.x"

// Toggle off or on
address |> Yeelight.toggle

// Switch off and on with no duration
address |> Yeelight.off Yeelight.Sudden 0
address |> Yeelight.on Yeelight.Sudden 0

// Switch off and on with a 10 second duration
address |> Yeelight.off Yeelight.Smooth 10000
address |> Yeelight.on Yeelight.Smooth 10000
```

The `Yeelight.Tests` project contains some integration tests for each operation.
