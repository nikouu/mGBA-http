# Examples

Looking for inspiration on what to use mGBA-http with? The following is a mix of:
- mGBA-http calls
- Scenarios
- Gameboy and Gameboy Advance games
- Programming languages and interfaces

As long as you can make an HTTP call, you can interact with mGBA-http.

_Note: There may be some differences with your experience in what mGBA or the scripting output looks like as time has moved on but the principles will be the same._

## 1. Moving and interacting

Using the C# console app mGBAHttpServer.TestClient included in this repository, we're able to move the main character around the overworld and interact with Pikachu in Pokémon Yellow.

| Scenario             | Game           | Caller         | Endpoint |
| -------------------- | -------------- | -------------- | -------- |
| Moving the character | Pokémon Yellow | C# console app | /button  |

https://github.com/nikouu/mGBA-http/assets/983351/48df2f5f-7f86-4edb-b2ef-e5856ea69912

### Code
See [mGBAHttpServer.TestClient](../src/mGBAHttpServer.TestClient).

## 2. Reading what nature a wild Pokémon has

Knowing what's ahead, or what secret things the game doesn't explicitly tell you is powerful. In this instance, we're using the in-built SwaggerUI functionality of mGBA-http to determine what nature the opposing wild Pokémon has in Pokémon Sapphire.

To get the nature of a wild Pokémon in Sapphire from memory:
1. Read the first dword (32 bits) starting from [address 0x030045C0](https://bulbapedia.bulbagarden.net/wiki/Pok%C3%A9mon_data_structure_(Generation_III)#Data_location). In this case we get decimal: 3121544013
2. Then do dword % 25. Here we get 3121544013 % 25 = 13
3. See where the number corresponds on the [list of natures](https://bulbapedia.bulbagarden.net/wiki/Nature#List_of_Natures). Here, 13 corrsponds to Jolly

We can validate this by checking the nature of the Pokémon just caught. In this case, the Wurmple is Jolly in nature.

| Scenario                                                | Game             | Caller    | Endpoint     |
| ------------------------------------------------------- | ---------------- | --------- | ------------ |
| Reading the memory location for opposing Pokémon nature | Pokémon Sapphire | SwaggerUI | /core/read32 |

https://github.com/nikouu/mGBA-http/assets/983351/c02e197a-db6f-4d35-a997-a5706c537fa9

### Code
N/A, uses SwaggerUI.

## 3. Modifying bag to have 50 bombs 
Modifying memory means you could do anything in game. Being invincible, unlocking everything, infinite amount of items, super speed, the list goes on. This example uses Postman to give the player 99 bombs in The Minish Cap. 

The bomb count on the top right of the game going from 35 to 50.

| Scenario                        | Game                                | Caller  | Endpoint     |
| ------------------------------- | ----------------------------------- | ------- | ------------ |
| Modifying memory for bomb count | The Legend of Zelda: The Minish Cap | Postman | /core/write8 |

https://github.com/nikouu/mGBA-http/assets/983351/95b9586e-2bb2-4e9d-93e2-dd818c6d3085

### Code
N/A, uses Postman.

## 4. Loading a savestate
Automating loading a savestate is easy. Perhaps your bot needs to reset the game to a known state. The following example is using Node.js to load a save file for Oracle of Seasons. 

| Scenario                     | Game                                   | Caller  | Endpoint           |
| ---------------------------- | -------------------------------------- | ------- | ------------------ |
| Loading a specific save file | The Legend of Zelda: Oracle of Seasons | Node.js | /core/loadstatefile |

https://github.com/nikouu/mGBA-http/assets/983351/edd2ca52-2fe1-43d7-a9cb-aab67cc79a20

### Code
```javascript
const http = require('http');

// url encoded path of: C:\mgba-http\ZELDA DIN.ss1
const options = {
  hostname: 'localhost',
  port: 5000,
  path: '/core/loadstatefile?path=C%3A%5Cmgba-http%5CZELDA%20DIN.ss1&flags=2',
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

## 5. Taking a screenshot
Perhaps your AI bot needs a way to see the game world. Maybe your Twitch audience just completed Exodia and they want to take a screenshot. The following takes a screenshot of a Yu-Gi-Oh duel in mGBA with Python.

| Scenario            | Game                                                       | Caller | Endpoint         |
| ------------------- | ---------------------------------------------------------- | ------ | ---------------- |
| Taking a screenshot | Yu-Gi-Oh! Worldwide Edition: Stairway to the Destined Duel | Python | /core/screenshot |

https://github.com/nikouu/mGBA-http/assets/983351/f6101b95-99c2-4f77-8ed6-527a61fd3be8

### Code
```python
import requests

# url encoded path of: C:\screenshots\ygo.png
url = "http://localhost:5000/core/screenshot?path=C%3A%5Cscreenshots%5Cygo.png"
response = requests.post(url)

print(response.text)
```

## 6. Getting frame count

A lot of the calls are simple GET requests, meaning we can even use the URL bar in a browser to successfully hit them. This example simply gets the number of frames that have passed since the Golden Sun ROM started, perhaps useful for waiting for an event, or approximating how long the ROM has been open for.

| Scenario                                    | Game                     | Caller           | Endpoint            |
| ------------------------------------------- | ------------------------ | ---------------- | ------------------- |
| Getting number of frames since ROM launched | Golden Sun: The Lost Age | Chrome (browser) | /core/currentframe  |

https://github.com/nikouu/mGBA-http/assets/983351/8ff0a205-a35f-4488-a59c-90c33c79f7e5

### Code
```
http://localhost:5000/core/getframecount
```

## 7. Checking game title

Each retail game has the game title in the header, here we use PowerShell (*???*) to get what this title is. Is it FireRed or is it LeafGreen?

| Scenario                       | Game            | Caller     | Endpoint           |
| ------------------------------ | --------------- | ---------- | ------------------ |
| Get game title from ROM header | Pokémon FireRed | PowerShell | /core/getgametitle |

https://github.com/nikouu/mGBA-http/assets/983351/d15b6f90-c18e-4550-aa5f-9013e302d715

### Code
```powershell
Invoke-WebRequest -URI http://localhost:5000/core/getgametitle
```
