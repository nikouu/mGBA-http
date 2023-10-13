# Implemented APIs

A table of which [mGBA scripting calls](https://mgba.io/docs/scripting.html) are reflected in mGBA-http. 

## Core

| mGBA call       | lua endpoint         | mGBA-http endpoint    |
| --------------- | -------------------- | --------------------- |
| addKey          | core.addKey          | /core/addKey          |
| addKeys         | core.addKeys         | /core/addKeys         |
| autoloadSave    | core.autoloadSave    | /core/autoloadSave    |
| checksum        | core.checksum        | /core/checksum        |
| clearKey        | core.checksum        | /core/clearKey        |
| clearKeys       | core.clearKeys       | /core/clearKeys       |
| currentFrame    | core.currentFrame    | /core/currentFrame    |
| frameCycles     | core.frameCycles     | /core/frameCycles     |
| frequency       | core.frequency       | /core/frequency       |
| getGameCode     | core.getGameCode     | /core/getGameCode     |
| getGameTitle    | core.getGameTitle    | /core/getGameTitle    |
| getKey          | core.getKey          | /core/getKey          |
| getKeys         | core.getKeys         | /core/getKeys         |
| loadFile        | core.loadFile        | /core/loadFile        |
| loadSaveFile    |                      |                       |
| loadStateBuffer |                      |                       |
| loadStateFile   |                      |                       |
| loadStateSlot   |                      |                       |
| platform        | core.platform        | /core/platform        |
| read16          | core.read16          | /core/read16          |
| read32          | core.read32          | /core/read32          |
| read8           | core.read8           | /core/read8           |
| readRange       |                      |                       |
| readRegister    | core.readRegister    | /core/readRegister    |
| reset           | core.reset           | /core/reset           |
| romSize         | core.romSize         | /core/romSize         |
| runFrame        | core.runFrame        | /core/runFrame        |
| saveStateBuffer | core.saveStateBuffer | /core/saveStateBuffer |
| saveStateFile   |                      |                       |
| saveStateSlot   |                      |                       |
| screenshot      | core.screenshot      | /core/screenshot      |
| setKeys         | core.setKeys         | /core/setKeys         |
| step            | core.step            | /core/step            |
| write16         |                      |                       |
| write32         |                      |                       |
| write8          |                      |                       |
| writeRegister   |                      |                       |

## Console

| mGBA call    | lua endpoint | mGBA-http endpoint |
| ------------ | ------------ | ------------------ |
| createBuffer |              |                    |
| error        | core.error   | /console/error     |
| log          | core.log     | /console/log       |
| warn         | core.warn    | /console/warn      |

## Memory Domain - GameBoy

| mGBA call | lua endpoint | mGBA-http endpoint |
| --------- | ------------ | ------------------ |
| base      |              |                    |
| bound     |              |                    |
| name      |              |                    |
| read16    |              |                    |
| read32    |              |                    |
| read8     |              |                    |
| readRange |              |                    |
| size      |              |                    |
| write16   |              |                    |
| write32   |              |                    |
| write8    |              |                    |

## Memory Domain - GameBoy Advance

| mGBA call | lua endpoint                       | mGBA-http endpoint            |
| --------- | ---------------------------------- | ----------------------------- |
| base      | memorydomain.gameboyadvance.base   | /memory/gameboyadvance/base   |
| bound     | memorydomain.gameboyadvance.bound  | /memory/gameboyadvance/bound  |
| name      | memorydomain.gameboyadvance.name   | /memory/gameboyadvance/name   |
| read16    | memorydomain.gameboyadvance.read16 | /memory/gameboyadvance/read16 |
| read32    | memorydomain.gameboyadvance.read32 | /memory/gameboyadvance/read32 |
| read8     | memorydomain.gameboyadvance.read8  | /memory/gameboyadvance/read8  |
| readRange |                                    |                               |
| size      | memorydomain.gameboyadvance.size   | /memory/gameboyadvance/size   |
| write16   |                                    |                               |
| write32   |                                    |                               |
| write8    |                                    |                               |

## Button - Custom API

| mGBA call | lua endpoint  | mGBA-http endpoint |
| --------- | ------------- | ------------------ |
|           | custom.button | /button/           |