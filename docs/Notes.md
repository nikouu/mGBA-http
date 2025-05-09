# Notes
These are notes that might be useful to share from development.

- Double checking memory calls: Tools > Game state views > View memory
- Pokemon stat addresses: https://bulbapedia.bulbagarden.net/wiki/Pok%C3%A9mon_data_structure_(Generation_III)
	- Explains more: https://www.ppnstudio.com/maker/PokemonMakerHelp.txt
	- Nature is the whole first 4 bytes of the Pokemon, then that value in decimal % 25 then the link above has the nature table
- Zelda bomb address: https://www.almarsguides.com/retro/walkthroughs/GBA/games/ZeldaTheMinishCap/Gameshark/
- Bitmasking has the keys from the scripting page in that order and each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select 
- For working GitHub embedded video uploads, ended up using the following via [this post](https://support.mozilla.org/en-US/questions/1379414)
```
ffmpeg -i in.mp4 -pix_fmt yuv420p -c:a copy -movflags +faststart out.mp4
```
- [Full list of socket errors](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socketerror?view=net-9.0)