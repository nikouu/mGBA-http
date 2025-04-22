# API Documentation

The following is generated from the [swagger.json](swagger.json) file via https://swagger-markdown-ui.netlify.app/ 
# mGBA-http
An HTTP interface for mGBA scripting.

**Contact information:**  
GitHub Repository  
https://github.com/nikouu/mGBA-http/  

### /mgba-http/button/tap

#### POST
##### Summary:

Sends button presses.

##### Description:

A custom convenience API that implements a key press and release. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [KeysEnum](#KeysEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /mgba-http/button/tapmany

#### POST
##### Summary:

Sends multiple button presses simultaneously.

##### Description:

A custom convenience API that implements multiple simultaneously keys being pressed and released. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keys | query | A key array. | Yes | [ [KeysEnum](#KeysEnum) ] |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /mgba-http/button/hold

#### POST
##### Summary:

Sends a held down button for a given duration in frames.

##### Description:

A custom convenience API that implements a held down button for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [KeysEnum](#KeysEnum) |
| duration | query | Duration in frames. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /mgba-http/button/holdmany

#### POST
##### Summary:

Sends multiple button presses simultaneously for a given duration in frames.

##### Description:

A custom convenience API that implements multiple simultaneously keys being pressed and released for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keys | query | A key array. | Yes | [ [KeysEnum](#KeysEnum) ] |
| duration | query | Duration in frames. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /console/error

#### POST
##### Summary:

Print an error to the console.

##### Description:

Presents textual information to the user via a console.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| message | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /console/log

#### POST
##### Summary:

Print a log to the console.

##### Description:

Presents textual information to the user via a console.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| message | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /console/warn

#### POST
##### Summary:

Print a warning to the console.

##### Description:

Presents textual information to the user via a console.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| message | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/addkey

#### POST
##### Summary:

Add a single key.

##### Description:

Add a single key to the currently active key list. As if the key were held down.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [KeysEnum](#KeysEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/addkeys

#### POST
##### Summary:

Add a bitmask of keys.

##### Description:

Add a bitmask of keys to the currently active key list. As if the keys were held down.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keyBitmask | query | Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/autoloadsave

#### POST
##### Summary:

Load save data.

##### Description:

Load the save data associated with the currently loaded ROM file.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/checksum

#### GET
##### Summary:

Get the checksum of the loaded ROM.

##### Description:

Get the checksum of the loaded ROM as base 10.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/clearkey

#### POST
##### Summary:

Remove a single key.

##### Description:

Remove a single key from the currently active key list. As if the key were released.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [KeysEnum](#KeysEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/clearkeys

#### POST
##### Summary:

Remove a bitmask of keys.

##### Description:

Remove a bitmask of keys from the currently active key list. As if the keys were released.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keyBitmask | query | Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/currentFrame

#### GET
##### Summary:

Get the number of the current frame.

##### Description:

Get the number of the current frame.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/framecycles

#### GET
##### Summary:

Get the number of cycles per frame.

##### Description:

Get the number of cycles per frame.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/frequency

#### GET
##### Summary:

Get the number of cycles per second.

##### Description:

Get the number of cycles per second.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/getgamecode

#### GET
##### Summary:

Get internal product code for the game.

##### Description:

Get internal product code for the game from the ROM header, if available.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/getgametitle

#### GET
##### Summary:

Get internal title of the game.

##### Description:

Get internal title of the game from the ROM header.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/getkey

#### GET
##### Summary:

Get the active state of a given key.

##### Description:

Get the active state of a given key.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [KeysEnum](#KeysEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/getkeys

#### GET
##### Summary:

Get the currently active keys as a bitmask.

##### Description:

Get the currently active keys as a bitmask.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/loadfile

#### POST
##### Summary:

Load a ROM file.

##### Description:

Load a ROM file into the current state of this core.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/loadsavefile

#### POST
##### Summary:

Load save data file.

##### Description:

Load save data from the given path. If the temporary flag is set, the given save data will not be written back to disk.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query |  | Yes | string |
| temporary | query |  | Yes | boolean |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/loadstatebuffer

#### POST
##### Summary:

Load state from a buffer.

##### Description:

Load state from a buffer.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| buffer | query |  | Yes | string |
| flags | query |  | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/loadstatefile

#### POST
##### Summary:

Load state from the given path.

##### Description:

Load state from the given path.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query |  | Yes | string |
| flags | query |  | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/loadstateslot

#### POST
##### Summary:

Load state from the slot number.

##### Description:

Load state from the slot number.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| slot | query |  | Yes | string |
| flags | query |  | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/platform

#### GET
##### Summary:

Get which platform is being emulated.

##### Description:

Get which platform is being emulated.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/read16

#### GET
##### Summary:

Read a 16-bit value from the given bus address.

##### Description:

Read a 16-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/read32

#### GET
##### Summary:

Read a 32-bit value from the given bus address.

##### Description:

Read a 32-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/read8

#### GET
##### Summary:

Read an 8-bit value from the given bus address.

##### Description:

Read an 8-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/readrange

#### GET
##### Summary:

Read byte range from the given offset.

##### Description:

Read byte range from the given offset and returns an array of unsigned integers

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| length | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/readregister

#### GET
##### Summary:

Read the value of the register with the given name.

##### Description:

Read the value of the register with the given name.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| regName | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/romsize

#### GET
##### Summary:

Get the size of the loaded ROM.

##### Description:

Get the size of the loaded ROM.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/runFrame

#### POST
##### Summary:

Run until the next frame.

##### Description:

Run until the next frame.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/savestatebuffer

#### POST
##### Summary:

Save state and return as a buffer.

##### Description:

Save state and returns an array of unsigned integers

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| flags | query |  | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/savestatefile

#### POST
##### Summary:

Save state to the given path.

##### Description:

Save state to the given path.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query |  | Yes | string |
| flags | query |  | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/savestateslot

#### POST
##### Summary:

Save state to the slot number.

##### Description:

Save state to the slot number.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| slot | query |  | Yes | string |
| flags | query |  | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/screenshot

#### POST
##### Summary:

Save a screenshot.

##### Description:

Save a screenshot.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query | For example, C:\screenshots\screenshot.png | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/setkeys

#### POST
##### Summary:

Set the currently active key list.

##### Description:

Set the currently active key list.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keys | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/step

#### POST
##### Summary:

Run a single instruction.

##### Description:

Run a single instruction.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/write16

#### POST
##### Summary:

Write a 16-bit value from the given bus address.

##### Description:

Write a 16-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| value | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/write32

#### POST
##### Summary:

Write a 32-bit value from the given bus address.

##### Description:

Write a 32-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| value | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/write8

#### POST
##### Summary:

Write an 8-bit value from the given bus address.

##### Description:

Write an 8-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| value | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /core/writeregister

#### POST
##### Summary:

Write the value of the register with the given name.

##### Description:

Write the value of the register with the given name.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| regName | query | Address in hex, e.g. 0x03000000 | Yes | string |
| value | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /coreadapter/reset

#### POST
##### Summary:

Reset the emulation.

##### Description:

Reset the emulation and calls the reset callback.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /coreadapter/memory

#### GET
##### Summary:

Get the platform specific set of MemoryDomains.

##### Description:

Gets the platform specific set of memory domains as an array of strings.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/base

#### GET
##### Summary:

Get the address of the base of this memory domain.

##### Description:

Get the address of the base of this memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/bound

#### GET
##### Summary:

Get the address of the end bound of this memory domain.

##### Description:

Get the address of the end bound of this memory domain. Note that this address is not in the domain itself, and is the address of the first byte past it.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/name

#### GET
##### Summary:

Get a short, human-readable name for this memory domain.

##### Description:

Get a short, human-readable name for this memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/read16

#### GET
##### Summary:

Read a 16-bit value from the given offset.

##### Description:

Read a 16-bit value from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/read32

#### GET
##### Summary:

Read a 32-bit value from the given offset.

##### Description:

Read a 32-bit value from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/read8

#### GET
##### Summary:

Read an 8-bit value from the given offset.

##### Description:

Read an 8-bit value from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/readrange

#### GET
##### Summary:

Read byte range from the given offset.

##### Description:

Read byte range from the given offset and returns an array of unsigned integers

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| length | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/size

#### GET
##### Summary:

Get the size of this memory domain in bytes.

##### Description:

Get the size of this memory domain in bytes.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/write16

#### POST
##### Summary:

Write a 16-bit value from the given offset.

##### Description:

Write a 16-bit value from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| value | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/write32

#### POST
##### Summary:

Write a 32-bit value from the given offset.

##### Description:

Write a 32-bit value from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| value | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### /memorydomain/write8

#### POST
##### Summary:

Write an 8-bit value from the given offset.

##### Description:

Write an 8-bit value from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query |  | Yes | string |
| address | query | Address in hex, e.g. 0x03000000 | Yes | string |
| value | query |  | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |
