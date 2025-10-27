# Roadmap

A rough, non-committal roadmap.

## Version 0.7.0

- ✅ Fully implement: 
    - `/core/loadfile`  (now as `/mgba-http/extension/loadfile`)
    - `/core/loadsavefile`
- ✅ Cleanup of project structure, example code, and load test code
- ✅ Docs for using just the Lua script
- ✅ Further integration tests
 
## Version 0.8.0

- ✅ Fully implement:
  - `/core/loadstatebuffer`
  - `/core/savestatebuffer`
- ✅ Explore the possibility of implementing `/core/runframe` (Expore done. [Dev says don't use `runFrame()`](https://discord.com/channels/453962671499509772/979634439237816360/1360317485596807179))
- ✅ Further integration tests

## Version 0.9.0

- Re-explore `TextBuffer`
- Re-explore how arrays are sent and recieved
  - `core.loadStateBuffer`
  - `core.savestatebuffer`
  - `core.readRange`
  - `memoryDomain.readRange`
  - `coreAdapter.memory`
  - `memoryDomain.readRange`
  - `mgba-http.button.addMany`
  - `mgba-http.button.clearMany`
  - `mgba-http.button.getAll`
  
- Consistent casing for Lua endpoint keys

## Version 0.10.0

- Upgrade to .NET 10
- Explore new AOT options
  - Including swapping out Swashbuckle
- ✅ Return to looking at loadfile and how that needs brackets at the moment to be parsed correctly
 
## Version 1.0.0

- Cleanup
