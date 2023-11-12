$projectFilePath = "mGBAHttpServer.csproj"
$xml = [xml](Get-Content $projectFilePath)

$version = $xml.Project.PropertyGroup.Version

$filenamePrefix = "mGBA-http-{0}" -f $version
$releaseArguments = "-c Release -f net8.0"
$selfContainedArguments = "--self-contained true -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true -p:DebuggerSupport=false"
$rids = @("win-x64", "linux-x64", "osx-x64")

$executablesDictionary = @{
  "win64-self-contained" = "-r win-x64 $($selfContainedArguments)"
}

foreach ($rid in $rids) {
    dotnet publish $projectFilePath $releaseArguments -p:AssemblyName="$($filenamePrefix)-$($rid)"
    dotnet publish $projectFilePath $releaseArguments -p:AssemblyName="$($filenamePrefix)-$($rid)-self-contained" $selfContainedArguments
}

$win64SelfContainedFilename = "mGBA-http-{0}-win64-self-contained" -f $version
dotnet publish .\mGBAHttpServer.csproj -c Release -f net8.0 -r win-x64 --self-contained true -p:PublishSingleFile=true -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true -p:DebuggerSupport=false -p:AssemblyName=$win64SelfContainedFilename

<#
$win64Filename = "mGBA-http-{0}-win64" -f $version
dotnet publish .\mGBAHttpServer.csproj -c Release -f net8.0 -r win-x64 --self-contained false  -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:DebuggerSupport=false -p:AssemblyName=$win64Filename
#>
