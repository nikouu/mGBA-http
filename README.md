# mGBA-http

mGBA-http exposes the mGBA scripting API via HTTP REST. So as long as you make HTTP requests, you'll be able to interact with mGBA. 

## Quick Installation
mGBA-http works with 0.10.2 of mGBA.

For the full installation, see the [full installation doc](docs/FullInstallAndGuide.md).

## Quick Guide

For the full guide, see the [full guide doc](docs/FullInstallAndGuide.md).

## Limitations
- No frame perfect calls. There is network latency between your application to mGBA-http and again latency between mGBA-http and mGBA. This will not be accurate for frame perfect manipulation and is meant for more general usage such as for "Twitch plays", AI playing bot, or other non frame specific application. For high accuracy manipulation see [Bizhawk](https://tasvideos.org/BizHawk/) which is used for TASBots.
- The following are not implemented in mGBA-http:
	- The Socket API
	- The CallbackManager API
- When very quickly sending requests, the requests may queue and be actioned on long after the input requests stop. Or in other words, no key inputs are eaten. For example, holding down on the d-pad with original hardware didn't trigger the down action long after d-pad release.
	- You may want to implement a rate limiter in your code. For instance, only send a request every x milliseconds and ignore input between. 

## Why?
This project came about because I didn't know lua and wanted to use C# control emulated Pokémon games in a cross platform way. I was there for and loved Twitch Plays Pokémon. Most GitHub projects for "X plays" style emulator controllers use the Windows SendKey API (or similar abstraction) regardless of language. 

When investigating how [Ironmon-Tracker](https://github.com/besteon/Ironmon-Tracker) worked after seeing it on [Arex's stream](https://www.twitch.tv/arex), I noticed [mGBA has a scripting API](https://mgba.io/docs/scripting.html) and more specifically a [socket API](https://mgba.io/docs/scripting.html#lua-root-socket) which I could interact with via C# and it went from there<sup>[[1]](https://github.com/nikouu/mGBA-lua-HelloWorld)</sup><sup>[[2]](https://github.com/nikouu/mGBA-lua-Socket)</sup>.

## Contributing
If you know lua, GameBoy/Advance, or mGBA specifics, I'd love for help. 

## Contact
If there's a problem feel free to start an issue, otherwise see [my about page](https://www.nikouusitalo.com/about/#contact) on how to contact me. 

## Acknowledgments
- The mGBA GitHub team for having socket examples
- [Zachary Handley](https://zachhandley.com/) for paving the way with [button press code](https://discord.com/channels/453962671499509772/979634439237816360/1124075643143995522)
- [heroldev/AGB-buttontest](https://github.com/heroldev/AGB-buttontest) for a simple button testing ROM

## Links
- [mGBA Website](https://mgba.io/)
- [mGBA GitHub](https://github.com/mgba-emu/mgba)
- [mGBA Discord](https://discord.gg/em2M2sG)

## License

