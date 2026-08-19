# Get version
$projectFilePath = "src\mGBAHttp\mGBAHttp.csproj"
$xml = [xml](Get-Content $projectFilePath)
$version = $xml.Project.PropertyGroup.Version[0]

# Enforce lua script
$luaVersionLine = Get-Content "mGBA-http.lua" | Where-Object { $_ -like '*local VERSION*' } | Select-Object -First 1
$luaVersion = $luaVersionLine.Split('"')[1]

if ($luaVersion -ne $version){
  throw "mGBA-http.lua version should be $($version). Currently is $($luaVersion)";
}

$luaLogLevelLine = Get-Content "mGBA-http.lua" | Where-Object { $_ -like '*local logLevel*' } | Select-Object -First 1
$luaLogLevel = $luaLogLevelLine.Split('=')[1].Trim()

if ($luaLogLevel -ne "2"){
  throw "mGBA-http.lua logLevel should be 2. Currently is $($luaLogLevel)";
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

  # Do not move the whole folder. Publish also emits an XML doc file, the IIS in-process handler
  # and static web asset manifests, and the release needs none of them.
  $binaryName = "$($filenamePrefix)-$($rid)"
  $published = @(Get-ChildItem ".\releaseStaging" -File | Where-Object { $_.Name -eq $binaryName -or $_.Name -eq "$($binaryName).exe" })

  if ($published.Count -ne 1) {
    throw "Publish for $rid did not produce $binaryName. Read the dotnet publish output above."
  }

  Move-Item -Path $published[0].FullName -Destination ".\release" -Force
}


# Copy over lua script and the config template.
# The release copy carries the version so a user can tell which script they loaded without opening it.
Copy-Item -Path ".\mGBA-http.lua" -Destination ".\release\$($filenamePrefix).lua" -Force
Copy-Item -Path ".\src\mGBAHttp\appsettings.json" -Destination ".\release" -Force

# Cleanup
Remove-Item .\releaseStaging -Recurse -Force