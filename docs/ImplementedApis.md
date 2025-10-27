# Implemented APIs

A table of which [mGBA scripting calls](https://mgba.io/docs/scripting.html) are reflected in mGBA-http. 

_Unstable_ APIs may not work as expected and may be fixed in a future update.

## Core

| mGBA call         | lua endpoint key     | mGBA-http endpoint                                   |
| ----------------- | -------------------- | ---------------------------------------------------- |
| addKey()          | core.addKey          | /core/addkey                                         |
| addKeys()         | core.addKeys         | /core/addkeys                                        |
| autoloadSave()    | core.autoloadSave    | /core/autoloadsave                                   |
| checksum()        | core.checksum        | /core/checksum                                       |
| clearKey()        | core.checksum        | /core/clearkey                                       |
| clearKeys()       | core.clearKeys       | /core/clearkeys                                      |
| currentFrame()    | core.currentFrame    | /core/currentframe                                   |
| frameCycles()     | core.frameCycles     | /core/framecycles                                    |
| frequency()       | core.frequency       | /core/frequency                                      |
| getGameCode()     | core.getGameCode     | /core/getgamecode                                    |
| getGameTitle()    | core.getGameTitle    | /core/getgametitle                                   |
| getKey()          | core.getKey          | /core/getkey                                         |
| getKeys()         | core.getKeys         | /core/getkeys                                        |
| loadFile()        | core.loadFile        | /core/loadfile (_Use /mgba-http/extension/loadfile_) |
| loadSaveFile()    | core.loadSaveFile    | /core/loadsavefile                                   |
| loadStateBuffer() | core.loadStateBuffer | /core/loadstatebuffer                                |
| loadStateFile()   | core.loadStateFile   | /core/loadstatefile                                  |
| loadStateSlot()   | core.loadStateSlot   | /core/loadstateslot                                  |
| platform()        | core.platform        | /core/platform                                       |
| read16()          | core.read16          | /core/read16                                         |
| read32()          | core.read32          | /core/read32                                         |
| read8()           | core.read8           | /core/read8                                          |
| readRange()       | core.readRange       | /core/readrange                                      |
| readRegister()    | core.readRegister    | /core/readregister                                   |
| reset()           | -                    | -                                                    |
| romSize()         | core.romSize         | /core/romsize                                        |
| runFrame()        | -                    | -                                                    |
| saveStateBuffer() | core.saveStateBuffer | /core/savestatebuffer                                |
| saveStateFile()   | core.saveStateFile   | /core/savestatefile                                  |
| saveStateSlot()   | core.saveStateSlot   | /core/savestateslot                                  |
| screenshot()      | core.screenshot      | /core/screenshot                                     |
| setKeys()         | core.setKeys         | /core/setkeys                                        |
| step()            | core.step            | /core/step                                           |
| write16()         | core.write16         | /core/write16                                        |
| write32()         | core.write32         | /core/write32                                        |
| write8()          | core.write8          | /core/write8                                         |
| writeRegister()   | core.writeRegister   | /core/writeregister                                  |

## CallbackManager
`CallbackManager` is not implemented in mGBA-http. 

## Console

| mGBA call      | lua endpoint key | mGBA-http endpoint |
| -------------- | ---------------- | ------------------ |
| createBuffer() | -                | -                  |
| error()        | core.error       | /console/error     |
| log()          | core.log         | /console/log       |
| warn()         | core.warn        | /console/warn      |

## CoreAdapter

| mGBA call | lua endpoint key   | mGBA-http endpoint  |
| --------- | ------------------ | ------------------- |
| reset()   | coreAdapter.reset  | /coreadapter/reset  |
| memory    | coreAdapter.memory | /coreadapter/memory |


## MemoryDomain

| mGBA call   | lua endpoint key       | mGBA-http endpoint      |
| ----------- | ---------------------- | ----------------------- |
| base()      | memoryDomain.base      | /memorydomain/base      |
| bound()     | memoryDomain.bound     | /memorydomain/bound     |
| name()      | memoryDomain.name      | /memorydomain/name      |
| read16()    | memoryDomain.read16    | /memorydomain/read16    |
| read32()    | memoryDomain.read32    | /memorydomain/read32    |
| read8()     | memoryDomain.read8     | /memorydomain/read8     |
| readRange() | memoryDomain.readRange | /memorydomain/readrange |
| size()      | memoryDomain.size      | /memorydomain/size      |
| write16()   | memoryDomain.write16   | /memorydomain/write16   |
| write32()   | memoryDomain.write32   | /memorydomain/write32   |
| write8()    | memoryDomain.write8    | /memorydomain/write8    |

## TextBuffer
`TextBuffer` is not implemented in mGBA-http. 

## Button - Custom API

Uses key letters as opposed to key IDs and bitmasks.

| mGBA call | lua endpoint key           | mGBA-http endpoint          |
| :-------: | -------------------------- | --------------------------- |
|     -     | mgba-http.button.add       | /mgba-http/button/add       |
|     -     | mgba-http.button.addMany   | /mgba-http/button/addmany   |
|     -     | mgba-http.button.clear     | /mgba-http/button/clear     |
|     -     | mgba-http.button.clearMany | /mgba-http/button/clearmany |
|     -     | mgba-http.button.get       | /mgba-http/button/get       |
|     -     | mgba-http.button.getAll    | /mgba-http/button/getall    |
|     -     | mgba-http.button.tap       | /mgba-http/button/tap       |
|     -     | mgba-http.button.tapMany   | /mgba-http/button/tapmany   |
|     -     | mgba-http.button.hold      | /mgba-http/button/hold      |
|     -     | mgba-http.button.holdMany  | /mgba-http/button/holdmany  |

## Extension - Custom API

| mGBA call | lua endpoint key             | mGBA-http endpoint            |
| :-------: | ---------------------------- | ----------------------------- |
|     -     | mgba-http.extension.loadFile | /mgba-http/extension/loadfile |
