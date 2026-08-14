# Removes empty static web assets files
function Remove-EmptyStaticWebAssetsFiles {
  param (
      [string]$folder
  )
  Get-ChildItem "$folder\*.staticwebassets.endpoints.json" | ForEach-Object {
      $content = Get-Content $_.FullName -Raw
      if ($content -eq '{"Version":1,"ManifestType":"Publish","Endpoints":[]}') {
          Remove-Item $_.FullName -Force
      }
  }
}

# Get version
$projectFilePath = "src\mGBAHttp\mGBAHttp.csproj"
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
$rids = @("win-x86","win-x64", "win-arm64", "linux-arm", "linux-arm64", "linux-x64", "osx-x64", "osx-arm64")

foreach ($folder in @(".\release", ".\releaseStaging")) {
  if (Test-Path $folder) {
      Remove-Item "$folder\*" -Recurse -Force -ErrorAction SilentlyContinue
  } else {
      New-Item -Path $folder -ItemType Directory | Out-Null
  }
}

# Create releases
foreach ($rid in $rids) {
  dotnet publish src\mGBAHttp\mGBAHttp.csproj -r $rid -c Release -p:SelfContained=true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o .\releaseStaging -p:AssemblyName="$($filenamePrefix)-$($rid)"

  Remove-EmptyStaticWebAssetsFiles -folder ".\releaseStaging"
  Move-Item -Path ".\releaseStaging\*.*" -Destination ".\release" -Force
}


# Copy over lua script
Copy-Item -Path ".\mGBASocketServer.lua" -Destination ".\release" -Force

# Cleanup
Remove-Item .\releaseStaging -Recurse -Force