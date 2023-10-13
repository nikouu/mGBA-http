# Implemented APIs

A table of which [mGBA scripting calls](https://mgba.io/docs/scripting.html) are reflected in mGBA-http. 

## Core

| mGBA call         | lua endpoint         | mGBA-http endpoint    |
| ----------------- | -------------------- | --------------------- |
| addKey()          | core.addKey          | /core/addkey          |
| addKeys()         | core.addKeys         | /core/addkeys         |
| autoloadSave()    | core.autoloadSave    | /core/autoloadsave    |
| checksum()        | core.checksum        | /core/checksum        |
| clearKey()        | core.checksum        | /core/clearkey        |
| clearKeys()       | core.clearKeys       | /core/clearkeys       |
| currentFrame()    | core.currentFrame    | /core/currentframe    |
| frameCycles()     | core.frameCycles     | /core/framecycles     |
| frequency()       | core.frequency       | /core/frequency       |
| getGameCode()     | core.getGameCode     | /core/getgamecode     |
| getGameTitle()    | core.getGameTitle    | /core/getgametitle    |
| getKey()          | core.getKey          | /core/getkey          |
| getKeys()         | core.getKeys         | /core/getkeys         |
| loadFile()        | core.loadFile        | /core/loadFile        |
| loadSaveFile()    |                      |                       |
| loadStateBuffer() |                      |                       |
| loadStateFile()   |                      |                       |
| loadStateSlot()   |                      |                       |
| platform()        | core.platform        | /core/platform        |
| read16()          | core.read16          | /core/read16          |
| read32()          | core.read32          | /core/read32          |
| read8()           | core.read8           | /core/read8           |
| readRange()       |                      |                       |
| readRegister()    | core.readRegister    | /core/readregister    |
| reset()           | core.reset           | /core/reset           |
| romSize()         | core.romSize         | /core/romsize         |
| runFrame()        | core.runFrame        | /core/runframe        |
| saveStateBuffer() | core.saveStateBuffer | /core/savestatebuffer |
| saveStateFile()   |                      |                       |
| saveStateSlot()   |                      |                       |
| screenshot()      | core.screenshot      | /core/screenshot      |
| setKeys()         | core.setKeys         | /core/setkeys         |
| step()            | core.step            | /core/step            |
| write16()         |                      |                       |
| write32()         |                      |                       |
| write8()          |                      |                       |
| writeRegister()   |                      |                       |

## Console

| mGBA call      | lua endpoint | mGBA-http endpoint |
| -------------- | ------------ | ------------------ |
| createBuffer() |              |                    |
| error()        | core.error   | /console/error     |
| log()          | core.log     | /console/log       |
| warn()         | core.warn    | /console/warn      |

## Memory Domain

| mGBA call   | lua endpoint        | mGBA-http endpoint |
| ----------- | ------------------- | ------------------ |
| base()      | memorydomain.base   | /memory/base       |
| bound()     | memorydomain.bound  | /memory/bound      |
| name()      | memorydomain.name   | /memory/name       |
| read16()    | memorydomain.read16 | /memory/read16     |
| read32()    | memorydomain.read32 | /memory/read32     |
| read8()     | memorydomain.read8  | /memory/read8      |
| readRange() |                     |                    |
| size()      | memorydomain.size   | /memory/size       |
| write16()   |                     |                    |
| write32()   |                     |                    |
| write8()    |                     |                    |

## TextBuffer
`TextBuffer` is not implemented in mGBA-http. 

| mGBA call    | lua endpoint | mGBA-http endpoint |
| ------------ | ------------ | ------------------ |
| advance()    |              |                    |
| clear()      |              |                    |
| cols()       |              |                    |
| getX()       |              |                    |
| getY()       |              |                    |
| moveCursor() |              |                    |
| print()      |              |                    |
| rows()       |              |                    |
| setName()    |              |                    |
| setSize()    |              |                    |

## Button - Custom API

| mGBA call | lua endpoint  | mGBA-http endpoint |
| --------- | ------------- | ------------------ |
|           | custom.button | /button            |