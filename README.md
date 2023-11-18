# mGBA-http

An HTTP interface for mGBA scripting. As long as you make HTTP requests, you'll be able to interact with mGBA. 

This project is great if you want to:
- Create a "Twitch Plays" 
- Write an AI bot in your preferred programming language
- Create an information dashboard for the current game (e.g. show your current Pokémon's stats like [Pokélink](https://twitter.com/pokelinkapp))
- Edit the memory of the running game

## Quick Start Guide
mGBA-http works with 0.10.2 of mGBA.

1. Ensure you have [mGBA](https://mgba.io/downloads.html)
1. Download mGBA-http and mGBASocketServer.lua from the [Releases](https://github.com/nikouu/mGBA-http/releases/latest) section 
1. Run mGBA-http
1. In mGBA, go to Tools > Scripting, then File > Load script and load in mGBASocketServer.lua

Once a ROM is laoded, you are now ready to start using mGBA-http.

For a more in-depth guide with pictures, see the [Full Guide](docs/FullGuide.md).

## Limitations
- No frame perfect calls. There is network latency between your application to mGBA-http and again latency between mGBA-http and mGBA. This will not be accurate for frame perfect manipulation and is meant for more general usage such as for "Twitch plays", AI playing bot, or other non frame specific application. For high accuracy manipulation see [Bizhawk](https://tasvideos.org/BizHawk/) which is used for TASBots.
- Not all scripting calls are implemented. See [ImplementedApis.md](docs/ImplementedApis.md) for the list of what is implemented.
- When very quickly sending requests, the requests may queue and be actioned on long after the input requests stop. Or in other words, no key inputs are eaten. For example, holding down on the d-pad with original hardware didn't trigger the down action long after d-pad release.
	- You may want to implement a rate limiter in your code. For instance, only send a request every x milliseconds and ignore input between. 

## Why?
This project came about because I didn't know lua and wanted to use C# control emulated Pokémon games in a cross platform way. I was there for and loved Twitch Plays Pokémon and looking around, most GitHub projects for "X plays" style emulator controllers use the Windows SendKey API (or similar abstraction) regardless of language. 

When investigating how [Ironmon-Tracker](https://github.com/besteon/Ironmon-Tracker) worked after seeing it on [Arex's stream](https://www.twitch.tv/arex), I noticed [mGBA has a scripting API](https://mgba.io/docs/scripting.html) and more specifically a [socket API](https://mgba.io/docs/scripting.html#lua-root-socket) which I could interact with via C# and it went from there<sup>[[1]](https://github.com/nikouu/mGBA-lua-HelloWorld)</sup><sup>[[2]](https://github.com/nikouu/mGBA-lua-Socket)</sup>.

## Contributing
If you know lua, GameBoy/Advance, or mGBA specifics, I'd love for help. 

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