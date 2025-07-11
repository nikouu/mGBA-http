# Full Guide (application)

This guide is for using the full mGBA-http application. If you only need to use the .lua script, see [Full Guide (lua script only)](/docs/FullGuide-lua.md).

## Setup

1. Ensure you have [mGBA](https://mgba.io/downloads.html)
1. Download both mGBA-http and `mGBASocketServer.lua` from the [Releases](https://github.com/nikouu/mGBA-http/releases/latest) section.
	- **Which mGBA-http?** 
		- If you have .NET installed download the smaller file type for your system. That is, the one without "self-contained" in the filename. 
		- If you do not have .NET installed, or are unsure, download the larger file type for your system. That is, the one with "self-contained" in the filename.
1. Run mGBA-http
![](Images/mgba-httpStart.jpg)
1. Open mGBA and click Tools > Scripting to open the Scripting window.
![](Images/ScripingMenuItem.jpg)
1. In the scripting window click File > Load script to bring up the file picker dialog.
![](Images/LoadScript.jpg)
1. Select the `mGBASocketServer.lua` file you downloaded earlier
1. Load up a ROM in mGBA
1. Done. mGBA is now ready to accept commands from mGBA-http.

![](Images/ReadyToGo.jpg)


## Usage

When running, mGBA-http presents as a mostly non-interactiable console, with the default start information of:
- The bound address. This is the root for the commands. Default: `http://localhost:5000`
- The SwaggerUI address. Default: `http://localhost:5000/index.html`
- The Swagger JSON address. Default: `http://localhost:5000/swagger/v1/swagger.json`

Log entries of what commands are being sent as well as errors will show up in this console.
![](Images/ConsoleOutputExample.jpg)

A quick way to begin to sending commands is heading to the SwaggerUI address and reqesting the ROM title as it requires no parameters:

![](Images/QuickStartExample.jpg)

See below for more examples, and the [mGBA scripting documentation](https://mgba.io/docs/scripting.html).

- To see what mGBA scripting APIs are implemented, see the [implemented APIs document](ImplementedApis.md).
- To see an overview of the swagger.json file, see the [API documentation](ApiDocumentation.md).
- Finally, the full [swagger.json](swagger.json) file.

### Configuration
If needed, there are minimal configuration points

#### appsettings.json
This file is **not** required for running mGBA-http normally. 

However, if you need further configuration, download this file from the releases page and put it in the same directory as mGBA-http. mGBA-http will pick up on these settings when run. See the [ASP.NET Core logging documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0#configure-logging) for more.

The following configuration is available:

| Config                                                | Notes                                                                |
| ----------------------------------------------------- | -------------------------------------------------------------------- |
| `Logging.LogLevel`                                    | Configure log levels                                                 |
| `Logging.LogFilters`                                  | Filter log levels                                                    |
| `Logging.Console.FormatterOptions.IncludeJsonDetails` | Includes a more full log entry in the console as a JSON string       |
| `Logging.Console.FormatterOptions.TimestampFormat`    | Update the timestamp format of the console log entries.              |
| `Kestrel.Endpoints`                                   | These are the mGBA-http listening ports                              |
| `mgba-http`                                           | These are the socket configurations for connecting mGBA-http to mGBA |

The image below shows to changes to the default settings:
1. `IncludeJsonDetails` to true
1. Update the mGBA-http listening ports

![](Images/UpdatedSettingsExample.jpg)


#### mGBASocketServer.lua

At the top, there is the `logLevel` flag. This will output timestamped logs to the scripting console based on the severity of the log entry. By default it is set to **2 - Information**:

| Value | Name        | Description  |
| ----- | ----------- | ------------ |
| 1     | Debug       | Debug logs   |
| 2     | Information | Info logs    |
| 3     | Warning     | Warning logs |
| 4     | Error       | Error logs   |
| 5     | None        | No logging   |

### Gotchas
- Make sure not to load the script twice. This causes issues with recieving data. If you need to reload the script, close and reopen mGBA then load up the script again. Closing and reopening the scripting window is not enough. 
- Most commands require a ROM being loaded into mGBA.
- Unless you change the ports in both appsettings.json and in mGBASocketServer.lua, you cannot run more than one instance of mGBA-http.


### Examples
1. See the [examples page](Examples.md).
1. There is a [C# test console project](/testing/) where you can send keys to mGBA.
1. The [CPU-Plays-Pokemon repo](https://github.com/nikouu/CPU-Plays-Pokemon).
