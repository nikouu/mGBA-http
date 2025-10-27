# API Documentation

The following is generated from the [swagger.json](swagger.json) file via https://swagger-markdown-ui.netlify.app/ 

# mGBA-http
An HTTP interface for mGBA scripting.

**Contact information:**  
GitHub Repository  
https://github.com/nikouu/mGBA-http/  

### /mgba-http/button/add

#### POST
##### Summary:

Adds a single button.

##### Description:

A custom convenience API that mimics /core/addkey but uses button names as opposed to their number value.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| button | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [ButtonEnum](#ButtonEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /mgba-http/button/clear

#### POST
##### Summary:

Remove a single button.

##### Description:

A custom convenience API that mimics /core/clearkey but uses button names as opposed to their number value.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| button | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [ButtonEnum](#ButtonEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response/ |

### /mgba-http/button/get

#### GET
##### Summary:

Get the active state of a given button.

##### Description:

A custom convenience API that mimics /core/getkey but uses button names as opposed to their number value.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| button | query | Button value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [ButtonEnum](#ButtonEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 0 if key is not pressed or 1 if the key is pressed. |

### /mgba-http/button/tap

#### POST
##### Summary:

Sends button presses.

##### Description:

A custom convenience API that implements a key press and release. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| button | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [ButtonEnum](#ButtonEnum) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response |

### /mgba-http/button/tapmany

#### POST
##### Summary:

Sends multiple button presses simultaneously.

##### Description:

A custom convenience API that implements multiple simultaneously keys being pressed and released. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keys | query | A key array. | Yes | [ [ButtonEnum](#ButtonEnum) ] |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response |

### /mgba-http/button/hold

#### POST
##### Summary:

Sends a held down button for a given duration in frames.

##### Description:

A custom convenience API that implements a held down button for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| button | query | Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L. | Yes | [ButtonEnum](#ButtonEnum) |
| duration | query | Duration in frames. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response |

### /mgba-http/button/holdmany

#### POST
##### Summary:

Sends multiple button presses simultaneously for a given duration in frames.

##### Description:

A custom convenience API that implements multiple simultaneously keys being pressed and released for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keys | query | A key array. | Yes | [ [ButtonEnum](#ButtonEnum) ] |
| duration | query | Duration in frames. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response |

### /console/error

#### POST
##### Summary:

Print an error to the console.

##### Description:

Print an error to the console. This will be shown as red text on light red background.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| message | query | The error message to display in console. | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /console/log

#### POST
##### Summary:

Print a log to the console.

##### Description:

Print a log to the console. This will be shown as regular text.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| message | query | The log message to display in console. | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /console/warn

#### POST
##### Summary:

Print a warning to the console.

##### Description:

Print a warning to the console. This will be shown as yellow text on light yellow background.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| message | query | The warning message to display in console. | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/addkey

#### POST
##### Summary:

Add a single key.

##### Description:

Add a single key to the currently active key list. As if the key were held down.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: 0-9 | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response |

### /core/addkeys

#### POST
##### Summary:

Add a bitmask of keys.

##### Description:

Add a bitmask of keys to the currently active key list. As if the keys were held down.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keyBitmask | query | The bitmask of keys to add. Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/autoloadsave

#### POST
##### Summary:

Load save data.

##### Description:

Load the save data associated with the currently loaded ROM file.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Boolean representing success. |

### /core/checksum

#### GET
##### Summary:

Get the checksum of the loaded ROM.

##### Description:

Get the checksum of the loaded ROM.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | The checksum. |

### /core/clearkey

#### POST
##### Summary:

Remove a single key.

##### Description:

Remove a single key from the currently active key list. As if the key were released.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: 0-9 | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/clearkeys

#### POST
##### Summary:

Remove a bitmask of keys.

##### Description:

Remove a bitmask of keys from the currently active key list. As if the keys were released.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keyBitmask | query | The bitmask of keys to clear. Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/currentFrame

#### GET
##### Summary:

Get the number of the current frame.

##### Description:

Get the number of the current frame.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Current frame number. |

### /core/framecycles

#### GET
##### Summary:

Get the number of cycles per frame.

##### Description:

Get the number of cycles per frame.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Cycles per frame. |

### /core/frequency

#### GET
##### Summary:

Get the number of cycles per second.

##### Description:

Get the number of cycles per second.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Cycles per second. |

### /core/getgamecode

#### GET
##### Summary:

Get internal product code for the game.

##### Description:

Get internal product code for the game from the ROM header, if available.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Product code. |

### /core/getgametitle

#### GET
##### Summary:

Get internal title of the game.

##### Description:

Get internal title of the game from the ROM header.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Game title. |

### /core/getkey

#### GET
##### Summary:

Get the active state of a given key.

##### Description:

Get the active state of a given key.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | query | Key value of: 0-9 | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 0 if key is not pressed or 1 if the key is pressed. |

### /core/getkeys

#### GET
##### Summary:

Get the currently active keys as a bitmask.

##### Description:

Get the currently active keys as a bitmask.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Bitmask string representing currently active keys. |

### /core/loadfile

#### POST
##### Summary:

Load a ROM file.

##### Description:

Load a ROM file into the current state of this core. **Note: Prefer using the /mgba-http/extension/loadfile endpoint.**

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query | Path to ROM file to load. | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

### /core/loadsavefile

#### POST
##### Summary:

Load save data file.

##### Description:

Load save data from the given path. If the temporary flag is set, the given save data will not be written back to disk.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query | Path to save file to load. | Yes | string |
| temporary | query | If true, save data will not be written back to disk. | Yes | boolean |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

### /core/loadstatebuffer

#### POST
##### Summary:

Load state from a buffer.

##### Description:

Load state from a hex array string buffer in the format: [d3,00,ea,66,...].

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| flags | query | State flags bitmask. Default 29 excludes screenshot flag (all except SCREENSHOT). Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16. | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

### /core/loadstatefile

#### POST
##### Summary:

Load state from the given path.

##### Description:

Load state from the given path.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query | Path to state file to load. | Yes | string |
| flags | query | State flags bitmask. Default 29 excludes screenshot flag. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16. | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

### /core/loadstateslot

#### POST
##### Summary:

Load state from the slot number.

##### Description:

Load state from the slot number.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| slot | query | State slot number to load from. | Yes | string |
| flags | query | State flags bitmask. Default 29 excludes screenshot flag. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16. | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

### /core/platform

#### GET
##### Summary:

Get which platform is being emulated.

##### Description:

Get which platform is being emulated.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Platform identifier. None: -1, GBA: 0, GB: 1. |

### /core/read16

#### GET
##### Summary:

Read a 16-bit value from the given bus address.

##### Description:

Read a 16-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300'). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 16-bit value of the read memory. |

### /core/read32

#### GET
##### Summary:

Read a 32-bit value from the given bus address.

##### Description:

Read a 32-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300'). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 32-bit value of the read memory. |

### /core/read8

#### GET
##### Summary:

Read an 8-bit value from the given bus address.

##### Description:

Read an 8-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300'). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 8-bit value of the read memory. |

### /core/readrange

#### GET
##### Summary:

Read byte range from the given offset.

##### Description:

Read byte range from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300'). | Yes | string |
| length | query | Number of bytes to read. | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Comma separated hex values of the read memory. |

### /core/readregister

#### GET
##### Summary:

Read the value of the register with the given name.

##### Description:

Read the value of the register with the given name.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| regName | query | Register name to read (e.g., 'r0', 'r1', 'sp', 'lr', 'pc'). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | The value of the register. |

### /core/romsize

#### GET
##### Summary:

Get the size of the loaded ROM.

##### Description:

Get the size of the loaded ROM.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | The ROM size. |

### /core/savestatebuffer

#### POST
##### Summary:

Save state and return as a buffer.

##### Description:

Save state and return as a buffer. This can be used with /core/loadstatebuffer to restore state.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| flags | query | State flags bitmask. Default 31 includes all flags (screenshot, metadata, etc). Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16. | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Comma separated hex values. |

### /core/savestatefile

#### POST
##### Summary:

Save state to the given path.

##### Description:

Save state to the given path.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query | Path where state file will be saved. | Yes | string |
| flags | query | State flags bitmask. Default 31 includes all flags. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16. | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

### /core/savestateslot

#### POST
##### Summary:

Save state to the slot number.

##### Description:

Save state to the slot number.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| slot | query | State slot number to save to. | Yes | string |
| flags | query | State flags bitmask. Default 31 includes all flags. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16. | No | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

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
| 200 | Empty success response. |

### /core/setkeys

#### POST
##### Summary:

Set the currently active key list.

##### Description:

Set the currently active key list.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| keys | query | The bitmask of keys to set as active. Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/step

#### POST
##### Summary:

Run a single instruction.

##### Description:

Run a single instruction.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/write16

#### POST
##### Summary:

Write a 16-bit value from the given bus address.

##### Description:

Write a 16-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x0300 | Yes | string |
| value | query | 16-bit decimal value to write (0-65535). | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/write32

#### POST
##### Summary:

Write a 32-bit value from the given bus address.

##### Description:

Write a 32-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x0300 | Yes | string |
| value | query | 32-bit decimal value to write. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/write8

#### POST
##### Summary:

Write an 8-bit value from the given bus address.

##### Description:

Write an 8-bit value from the given bus address.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| address | query | Address in hex, e.g. 0x0300 | Yes | string |
| value | query | 8-bit decimal value to write (0-255). | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /core/writeregister

#### POST
##### Summary:

Write the value of the register with the given name.

##### Description:

Write the value of the register with the given name.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| regName | query | Register name to write (e.g., 'r0', 'r1', 'sp', 'lr', 'pc', etc). | Yes | string |
| value | query | Decimal value to write to the register. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /coreadapter/reset

#### POST
##### Summary:

Reset the emulation.

##### Description:

Reset the emulation and calls the reset callback.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /coreadapter/memory

#### GET
##### Summary:

Get the platform specific set of memory domains.

##### Description:

Get the platform specific set of memory domains.

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | The memory domains as a comma separated string. |

### /mgba-http/extension/loadfile

#### POST
##### Summary:

Load a ROM file.

##### Description:

Load a ROM file into the current state of this core. This convenience API handles the ROM load and reset of the emulator.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| path | query | Path to ROM file to load. | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success status as a boolean. |

### /memorydomain/base

#### GET
##### Summary:

Get the base address of the memory domain.

##### Description:

Get the base address of the specified memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Address of the base of this memory domain. |

### /memorydomain/bound

#### GET
##### Summary:

Get the bound of the memory domain.

##### Description:

Get the address of the end bound of this memory domain. Note that this address is not in the domain itself, and is the address of the first byte past it.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Bound address as a string. |

### /memorydomain/name

#### GET
##### Summary:

Get the name of the memory domain.

##### Description:

Get a short, human-readable name for this memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Memory domain name. |

### /memorydomain/read16

#### GET
##### Summary:

Read a 16-bit value from the given offset.

##### Description:

Read a 16-bit value from the given offset in the specified memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300') | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 16-bit value as a string. |

### /memorydomain/read32

#### GET
##### Summary:

Read a 32-bit value from the given offset.

##### Description:

Read a 32-bit value from the given offset in the specified memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300') | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 32-bit value as a string. |

### /memorydomain/read8

#### GET
##### Summary:

Read an 8-bit value from the given offset.

##### Description:

Read an 8-bit value from the given offset in the specified memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300') | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | 8-bit value as a string. |

### /memorydomain/readrange

#### GET
##### Summary:

Read byte range from the given offset.

##### Description:

Read byte range from the given offset.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |
| address | query | Address as hex string (e.g., '0x0300'). | Yes | string |
| length | query | Number of bytes to read. | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Comma separated hex values. |

### /memorydomain/size

#### GET
##### Summary:

Get the size of the memory domain.

##### Description:

Get the size of this memory domain in bytes.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Size in bytes as a string. |

### /memorydomain/write16

#### POST
##### Summary:

Write a 16-bit value to the given offset.

##### Description:

Write a 16-bit value to the given offset in the specified memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300'). | Yes | string |
| value | query | 16-bit decimal value to write (0-65535). Not hex. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /memorydomain/write32

#### POST
##### Summary:

Write a 32-bit value to the given offset.

##### Description:

Write a 32-bit value to the given offset in the specified memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300'). | Yes | string |
| value | query | 32-bit decimal value to write. Not hex. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |

### /memorydomain/write8

#### POST
##### Summary:

Write an 8-bit value to the given offset.

##### Description:

Write an 8-bit value to the given offset in the specified memory domain.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| memoryDomain | query | Memory domain name (e.g., 'wram', 'cart0', 'bios', etc). | Yes | string |
| address | query | Address as hex string with 0x prefix (e.g., '0x0300'). | Yes | string |
| value | query | 8-bit decimal value to write (0-255). Not hex. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Empty success response. |
