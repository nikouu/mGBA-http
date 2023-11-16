# Examples

Looking for inspiration on what to use mGBA-http with? The following is a mix of:
- mGBA-http calls
- Scenarios
- Gameboy and Gameboy Advance games
- Programming languages and interfaces

As long as you can make an HTTP call, you can interact with mGBA-http.

## Moving and interacting

Using the C# console app mGBAHttpServer.TestClient included in this repository, we're able to move the main character around the overworld and interact with Pikachu in Pokémon Yellow.

| Scenario             | Game           | Caller         | Endpoint |
| -------------------- | -------------- | -------------- | -------- |
| Moving the character | Pokémon Yellow | C# console app | /button  |

### Code
See [mGBAHttpServer.TestClient](../src/mGBAHttpServer.TestClient).

## Reading what nature a wild Pokémon has

Knowing what's ahead, or what secret things the game doesn't explicitly tell you is powerful. In this instance, we're using the in-built SwaggerUI functionality of mGBA-http to determine what nature the opposing wild Pokémon has in Pokémon Sapphire.

| Scenario                                                | Game             | Caller    | Endpoint |
| ------------------------------------------------------- | ---------------- | --------- | -------- |
| Reading the memory location for opposing Pokémon nature | Pokémon Sapphire | SwaggerUI |          |

### Code
N/A, uses SwaggerUI.

## Modifying bag to have 99 bombs 
Modifying memory means you could do anything in game. Being invincible, unlocking everything, infinite amount of items, super speed, the list goes on. This example uses Postman to give the player 99 bombs in The Minish Cap.

| Scenario                        | Game                                | Caller  | Endpoint |
| ------------------------------- | ----------------------------------- | ------- | -------- |
| Modifying memory for bomb count | The Legend of Zelda: The Minish Cap | Postman |          |

### Code
N/A, uses Postman.

## Loading a save
Automating loading a save after a savestate goes awry. Perhaps your bot needs to reset the game to a known state. The following example is using Node.js to load a save file for Oracle of Seasons. 

| Scenario                     | Game                                   | Caller  | Endpoint           |
| ---------------------------- | -------------------------------------- | ------- | ------------------ |
| Loading a specific save file | The Legend of Zelda: Oracle of Seasons | Node.js | /core/loadsavefile |

### Code
```javascript
const http = require('http');

// url encoded path of: c:\saves\ZELDA DIN.sav
const options = {
  hostname: 'localhost',
  port: 5000,
  path: '/core/loadsavefile?path=c%3A%5Csaves%5CZELDA%20DIN.sav&temporary=true',
  method: 'POST'
};

const req = http.request(options, response => {
  response.on('data', chunk => {
    console.log(chunk);
  });
});

req.on('error', error => {
  console.error(error);
});

req.end();
```

## Taking a screenshot
Perhaps your AI bot needs a way to see the game world. Maybe your Twitch audience just completed Exodia and they want to take a screenshot. The following takes a screenshot of a Yu-Gi-Oh duel in mGBA with Python.

| Scenario            | Game                                                       | Caller | Endpoint         |
| ------------------- | ---------------------------------------------------------- | ------ | ---------------- |
| Taking a screenshot | Yu-Gi-Oh! Worldwide Edition: Stairway to the Destined Duel | Python | /core/screenshot |

### Code
```python
import requests

# url encoded path of: c:\screenshots
url = "http://localhost:5000/core/screenshot?path=c%3A%5Cscreenshots"
response = requests.post(url)

print(response.text)
```

## Getting frame count

A lot of the calls are simple GET requests, meaning we can even use the URL bar in a browser to successfully hit them. This example simply gets the number of frames that have passed since the Golden Sun ROM started, perhaps useful for waiting for an event, or approximating how long the ROM has been open for.

| Scenario                                    | Game                     | Caller           | Endpoint            |
| ------------------------------------------- | ------------------------ | ---------------- | ------------------- |
| Getting number of frames since ROM launched | Golden Sun: The Lost Age | Chrome (browser) | /core/getframecount |

### Code
```
http://localhost:5000/core/getframecount
```

## Checking game title

Each retail game has the game title in the header, here we use PowerShell (*???*) to get what this title is. Is it FireRed or is it LeafGreen?

| Scenario                       | Game            | Caller     | Endpoint           |
| ------------------------------ | --------------- | ---------- | ------------------ |
| Get game title from ROM header | Pokémon FireRed | PowerShell | /core/getgametitle |

### Code
```powershell
Invoke-WebRequest -URI http://localhost:5000/core/getgametitle
```