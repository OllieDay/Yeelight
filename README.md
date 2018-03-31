# Yeelight

Library for controlling Xiaomi Yeelight Smart LED Bulb.

## Getting started

Install the NuGet package into your application.

### Package Manager

```
Install-Package Yeelight
```

### .NET CLI

```
dotnet add package Yeelight
```

## Usage

All functions take an `IPAddress` for the target light.
```fsharp
let address = IPAddress.Parse "192.168.0.x"
```

Many functions take an `Effect` that determines the behaviour of an operation.
```fsharp
type Effect =
	// Instantaneous with no delay
	| Sudden
	// Gradual with the specified duration in milliseconds
	| Smooth of uint32
```

All functions return a `Response` with the value `Ok` for a successful request, or an `Error` that contains a message.
```fsharp
type Response = Ok | Error of string
```

### Power

```fsharp
type Power = Off | On
```

```fsharp
// Set power
address |> Yeelight.setPower Yeelight.Off Yeelight.Sudden
address |> Yeelight.setPower Yeelight.On Yeelight.Sudden

// Switch off and on
address |> Yeelight.off Yeelight.Sudden
address |> Yeelight.on Yeelight.Sudden

// Toggle off and on
address |> Yeelight.toggle
```

### Brightness

```fsharp
// Percentage: 1 - 100
type Brightness = int
```

```fsharp
// Set brightness to 50%
address |> Yeelight.setBrightness 50 Yeelight.Sudden
```

### Temperature

```fsharp
// Kelvin: 1700 - 6500
type Temperature = int
```

```fsharp
// Set temperature to 3500 K
address |> Yeelight.setTemperature 3500 Yeelight.Sudden
```

### Color

Color can be set using either RGB or HSV.

```fsharp
// 0 - 255
type Red = int
type Green = int
type Blue = int

// 0 - 359
type Hue = int

// 0 - 100
type Saturation = int

type Color =
	// Triple of red, green, blue
	| Rgb of Red * Green * Blue
	// Tuple of hue, saturation
	| Hsv of Hue * Saturation
```

```fsharp
// Set color to red = 255, green = 0, blue = 127
address |> Yeelight.setColor (Yeelight.Rgb (255, 0, 127)) Yeelight.Sudden

// Set color to hue = 180, saturation = 50
address |> Yeelight.setColor (Yeelight.Hsv (180, 50)) Yeelight.Sudden
```

### Name

```fsharp
type Name = string
```

```fsharp
// Set name to "Bedroom"
address |> Yeelight.setName "Bedroom"
```

__Note:__ When using the Yeelight official app, the device name is stored in the cloud. This method instead stores the name in device's persistent memory, so the two names may differ.
