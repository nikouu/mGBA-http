# mGBA-http

An HTTP interface for mGBA scripting. As long as you make HTTP requests, you'll be able to interact with mGBA.

![](docs/Images/FlowDiagram.jpg)

This project is great if you want to:
- Create a "Twitch Plays" 
- Write an AI bot in your preferred programming language
- Create an information dashboard for the current game (e.g. show your current Pokémon's stats like [Pokélink](https://twitter.com/pokelinkapp))
- Edit the memory of the running game

In the example below, a C# program _[mGBAHttpServer.TestClient](../src/mGBAHttpServer.TestClient)_ 
- (top left) is accepting key inputs from keyboard and sending them to mGBA-http
- (top right) mGBA-http is forwarding these to mGBA
- (bottom left) mGBA's built-in scripting API
- (bottom right) then the key inputs are read by the ROM

If you want to see more real examples with videos and explanations, check out the [examples page](docs/Examples.md).

https://github.com/nikouu/mGBA-http/assets/983351/08907201-b437-4930-a769-ee441fbdde0d


## Quick Start Guide
mGBA-http works with 0.10.2 of mGBA.

1. Ensure you have [mGBA](https://mgba.io/downloads.html)
1. Download mGBA-http and mGBASocketServer.lua from the [Releases](https://github.com/nikouu/mGBA-http/releases/latest) section 
1. Run mGBA-http
1. In mGBA, go to Tools > Scripting, then File > Load script and load in mGBASocketServer.lua

Once a ROM is loaded, you are now ready to start using mGBA-http.

For a more in-depth guide with pictures, see the [Full Guide](docs/FullGuide.md).

## Limitations
- No frame perfect calls. There is network latency between your application to mGBA-http and again latency between mGBA-http and mGBA. This will not be accurate for frame perfect manipulation and is meant for more general usage such as for "Twitch plays", AI playing bot, or other non frame specific application. For high accuracy manipulation see [Bizhawk](https://tasvideos.org/BizHawk/) which is used for TASBots.
- Not all scripting calls are implemented. See [ImplementedApis.md](docs/ImplementedApis.md) for the list of what is implemented.
- When very quickly sending requests, the requests may queue and be actioned on long after the input requests stop. Or in other words, no key inputs are eaten. For example, holding down on the d-pad with original hardware didn't trigger the down action long after d-pad release.
	- You may want to implement a rate limiter in your code. For instance, only send a request every x milliseconds and ignore input between. 

## Why?
This project came about because I didn't know lua and wanted to use C# control emulated Pokémon games in a cross platform way. I was there for and loved Twitch Plays Pokémon and looking around, most GitHub projects for "X plays" style emulator controllers use the Windows SendKey API (or similar abstraction) regardless of language. 

When investigating how [Ironmon-Tracker](https://github.com/besteon/Ironmon-Tracker) worked after seeing it on [Arex's stream](https://www.twitch.tv/arex), I noticed [mGBA has a scripting API](https://mgba.io/docs/scripting.html) and more specifically a [socket API](https://mgba.io/docs/scripting.html#lua-root-socket) which I could interact with via C# and it went from there<sup>[[1]](https://github.com/nikouu/mGBA-lua-HelloWorld)</sup><sup>[[2]](https://github.com/nikouu/mGBA-lua-Socket)</sup>.

mGBA-http builds on the mGBA socket API abstraction and adds an HTTP layer to simplify interaction and development - which is exactly what I want for future projects.

## Contributing
If you know Lua, GameBoy/Advance, or mGBA specifics, I'd love for help. 

### Development

If you're a .NET developer, the setup simple and familiar opening the [solution file](src/CmGBAHttpServer.sln) using Visual Studio (At least VS 17.9) and .NET 8. 

If you're a .NET developer sans Visual Studio or Windows, you'll also feel the setup is simple and familiar with the [solution file](src/CmGBAHttpServer.sln).

If you're not a .NET developer, check out the comprehensive [C# learning website](https://dotnet.microsoft.com/en-us/learn/csharp) from Microsoft. You can program in C# on whatever platform whether it's Windows, Mac, or Linux.

#### Design Document

The rough design philosphy is outlined in the [design document](docs/Design.md). Please understand and follow this when considering a contribution.

### Build

The PowerShell release script [ReleaseBuild.ps1](ReleaseBuild.ps1) creates the final binaries. PowerShell is cross platform and can be downloaded via the [PowerShell download documentation](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.3).

The script generates binaries closely aligned with the operating systems and architectures that [mGBA provides downloads](https://mgba.io/downloads.html) for.

#### AOT

To reduce the barrier of entry, mGBA-http also has [self-contained](https://learn.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained) builds using Ahead of Time Compilation. These are the larger binaries with "self-contained" in the filename and bring the entirity of .NET needed to run the executable - meaning the user does not need to download the [.NET runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) to use mGBA-http.

## Contact
If there's a problem feel free to start an issue, otherwise see [my about page](https://www.nikouusitalo.com/about/#contact) on how to contact me. 



## Acknowledgments
- The mGBA GitHub team for having socket examples
- [Zachary Handley](https://zachhandley.com/) for paving the way with [button press code](https://discord.com/channels/453962671499509772/979634439237816360/1124075643143995522)
- [heroldev/AGB-buttontest](https://github.com/heroldev/AGB-buttontest) for a simple button testing ROM

[References.md](docs/References.md) has useful links during development.

This project is not associated to the development or development team of mGBA. I'm just a fan ✌

## Links
- [mGBA Website](https://mgba.io/)
- [mGBA GitHub](https://github.com/mgba-emu/mgba)
- [mGBA Discord](https://discord.gg/em2M2sG)
- [My Website](https://www.nikouusitalo.com/)

## License
[MIT](LICENSE)

If you end up using mGBA-http, [drop me a message](https://www.nikouusitalo.com/about/#contact) and tell me what you're up to!
