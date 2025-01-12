# ClipEditor
[![GitHub release](https://img.shields.io/github/release/ImVertil/ClipEditor.svg)](https://github.com/ImVertil/ClipEditor/releases/latest) \
An application that allows you to cut the best moments from your videos/clips via FFmpeg with a simple user interface. \
<br/>
This is a personal project that I work on at my own pace. New features will be added as I see fit, based mostly on my needs.

## Showcase
[![Watch the showcase here](https://img.youtube.com/vi/E_NA8-V7aww/0.jpg)](https://www.youtube.com/watch?v=E_NA8-V7aww)

## Libraries/Tools used
- [Xabe.FFmpeg](https://ffmpeg.xabe.net/)
- [FFmpeg](https://www.ffmpeg.org/)
- [WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit)

## Requirements
- A device with Windows operating system,
- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

## Build
If you want to build the application yourself:
- Clone the repository and open the project in Visual Studio,
- Make sure `Xabe.FFmpeg` and `Extended.Wpf.Toolkit` packages are installed via NuGet,
- After you build the application, make sure you either put `ffmpeg.exe` and `ffprobe.exe` in the Resources folder or have FFmpeg configured in `PATH`.
