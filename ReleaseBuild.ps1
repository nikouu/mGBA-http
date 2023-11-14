$projectFilePath = "src\mGBAHttpServer\mGBAHttpServer.csproj"
$xml = [xml](Get-Content $projectFilePath)

$version = $xml.Project.PropertyGroup.Version

$filenamePrefix = "mGBA-http-{0}" -f $version
$rids = @("win-x64")
#, "linux-x64", "osx-x64"
foreach ($rid in $rids) {
    dotnet publish src\mGBAHttpServer\mGBAHttpServer.csproj -p:AssemblyName="$($filenamePrefix)-$($rid)" -p:PublishSingleFile=true --self-contained=false
    #dotnet publish src\mGBAHttpServer\mGBAHttpServer.csproj -r $rid -p:AssemblyName="$($filenamePrefix)-$($rid)-self-contained" --self-contained=true -p:PublishSingleFile=true -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true -p:DebuggerSupport=false
}