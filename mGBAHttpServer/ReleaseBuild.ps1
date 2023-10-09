$version = "0.0.0"

$win64SelfContainedFilename = "mGBA-http-{0}-win64-self-contained" -f $version
dotnet publish .\mGBAHttpServer.csproj -c Release -f net8.0 -r win-x64 --self-contained true  -p:PublishSingleFile=true -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true -p:DebuggerSupport=false -p:AssemblyName=$win64SelfContainedFilename

<#
$win64Filename = "mGBA-http-{0}-win64" -f $version
dotnet publish .\mGBAHttpServer.csproj -c Release -f net8.0 -r win-x64 --self-contained false  -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:DebuggerSupport=false -p:AssemblyName=$win64Filename
#>