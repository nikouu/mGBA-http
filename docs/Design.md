# Design

This document outlines the rough design philosophies for mGBA-http. 

## Goal

The goal of mGBA-http is to simplify developer/user experience by programmatically interfacing with mGBA in a cross platform way via HTTP.

## Target Audience

mGBA-http is developed for the following audience personas:

1. A software developer
	- Either integrating mGBA-http into their own project, or developing on mGBA-http.
	- Specifically a .NET developer of intermediate or higher level
	- However with the simplicity of the code, it should be easy enough for another intermediate or higher language developer to at least understand the control flow by reading the code.
3. Non-software developer following instructions
	- It should be easy enough to setup for non-software developer. 
	- Coding ability should not be required to use mGBA-http. 
	- Users should be able to do everything they require from mGBA-http with a few clicks in a familiar way by following a short instructional guide. E.g. mGBA-http is one part of a larger setup that involves mGBA and whatever may send requests to mGBA-http.

## Simplicity

Simplicity concerns both personas described above.

Developer:
- mGBA-http is just a thin wrapper around the mGBA socket API.
- Data transformations should be at a minimum.
- mGBA-http calls should as closely as possible map 1 to 1 with the mGBA socket API. 
- Code does not need needless abstractions or overengineering. 
- It needs to remain readable and workable by an intermediate skill level .NET developer as long as they're up to date with features.
- Unless there is a compelling reason, the UI will be a simple console.
	- The UI for the bundled in SwaggerUI for quick prototyping is outside of the scope of this document.
- The Lua script should remain in a single file to ease file management for the end user.
- The PowerShell build script should remain accessible, easy to run, easy to understand, and quickly modifiable. 

The exception is the custom button API:
- The mGBA key API works well for the emulator but it requires different thinking from the user. The variability for the HTTP call times meaning a key may stay down or up longer than expected and prove frustrating for the user. This is why there is a custom button API. 

There may be more custom APIs in the future if needs must.

Non-software developer user:
- mGBA-http should be familiar enough to the user that we can rely on affordances. 
- The user should get all they need to know from a simple guide with pictures.

## Cross Platform

mGBA exists across Windows, Mac, and Linux (technically more too) and as such, mGBA-http too needs to be cross platform. This means no platform specific API calls will be made in mGBA-http.

mGBA-http builds will as close as possible target operating systems and architectures that mGBA does.

The Lua script is cross platform by default as it's consumed by cross platform mGBA. 

The PowerShell script is cross platform as PowerShell itself is cross platform.

## Accessibility 

Accessibility for this purpose relates to how easy it is to pick up mGBA-http and both use it and develop for it. 

As mentioned earlier in this document, mGBA-http needs to remain readable and workable by intermediate or higher skill level .NET developer as long as they're up to date with features.

Similarly to *Cross Platform* above if a developer wants to clone/fork the code and develop in a non-Windows or non-Visual Studio environment, they should have the freedom to do so. 

It should be easy to develop against mGBA-http. Unless there is a compelling reason, just GET and POST calls should be used. Swagger is included in this project both as the `swagger.json` file and the interactive SwaggerUI to help and ease protyping.

## Languages

While mGBA-http is talked about as a singular piece, it's made up of both the C# server and the Lua script that loads into mGBA. 

Both the C# and Lua components of mGBA-http should adhere to this document and continue to be C# and Lua.

The PowerShell build script should also adhere to this document.

## Limitations

There are accepted limitations to mGBA-http. 

- It takes time for an HTTP request from origin to mGBA-http then time again from mGBA-http to mGBA. 
	- This makes mGBA-http unsuitable for frame sensitive inputs.
- Not all mGBA API calls are simple and may return complex objects. Unless there is a complelling reason, these will not be implemented.
- Many quickly sent requests may queue.
	- For inputs this may mean the key inputs happen long after the user has stopped them.
