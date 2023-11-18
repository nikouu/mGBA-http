# Get version
$projectFilePath = "src\mGBAHttpServer\mGBAHttpServer.csproj"
$xml = [xml](Get-Content $projectFilePath)
$version = $xml.Project.PropertyGroup.Version[0]

# Enforce lua script 
$luaVersionLine = (Get-Content "mGBASocketServer.lua")[2];
$luaVersion = ($luaVersionLine -split ' ')[2].Trim()

if ($luaVersion -ne $version){
  throw "mGBASocketServer.lua version should be $($version). Currently is $($luaVersion)";
}

# Setup publish variables
$filenamePrefix = "mGBA-http-{0}" -f $version
$rids = @("win-x64", "linux-x64", "osx-x64")

# Clean folder
Remove-Item .\release\* -Recurse -Force

# Create releases
foreach ($rid in $rids) {
  dotnet publish src\mGBAHttpServer\mGBAHttpServer.csproj -r $rid --self-contained=false -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o .\releaseStaging -p:AssemblyName="$($filenamePrefix)-$($rid)"  
  Move-Item -Path ".\releaseStaging\*.*" -Destination ".\release" -Force

  dotnet publish src\mGBAHttpServer\mGBAHttpServer.csproj -r $rid --self-contained=true  -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o .\releaseStaging -p:AssemblyName="$($filenamePrefix)-$($rid)-self-contained" -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true
  Move-Item -Path ".\releaseStaging\*.*" -Destination ".\release" -Force
}

# Copy over lua script
Copy-Item -Path ".\mGBASocketServer.lua" -Destination ".\release" -Force

# Cleanup
Remove-Item .\releaseStaging -Recurse -Force