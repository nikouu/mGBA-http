$projectFilePath = "src\mGBAHttpServer\mGBAHttpServer.csproj"
$xml = [xml](Get-Content $projectFilePath)

$version = $xml.Project.PropertyGroup.Version

$filenamePrefix = "mGBA-http-{0}" -f $version
$selfContainedArguments = "--self-contained=true -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true -p:DebuggerSupport=false"
$rids = @("win-x64", "linux-x64", "osx-x64")
# , "linux-x64", "osx-x64"
foreach ($rid in $rids) {
    Write-Host "dotnet publish ${projectFilePath} -p:AssemblyName=""$($filenamePrefix)-$($rid)"""

    #& dotnet publish @(${projectFilePath}, "-p:AssemblyName=""$($filenamePrefix)-$($rid)""")
    #& dotnet publish @(${projectFilePath}, "-p:AssemblyName=""$($filenamePrefix)-$($rid)""", ${selfContainedArguments})
    #Invoke-Expression "dotnet publish ${$projectFilePath} ${$releaseArguments} ${$selfContainedArguments} -p:AssemblyName='$($filenamePrefix)-$($rid)'"

    #dotnet publish @($projectFilePath, "-p:AssemblyName=""$($filenamePrefix)-$($rid)""")

    #dotnet publish @($projectFilePath, $selfContainedArguments, "-p:AssemblyName=""$($filenamePrefix)-$($rid)""")


    #dotnet publish $projectFilePath -p:AssemblyName="$($filenamePrefix)-$($rid)-self-contained" $selfContainedArguments

    #dotnet publish @("src\mGBAHttpServer\mGBAHttpServer.csproj", "--self-contained:true") 

    dotnet publish src\mGBAHttpServer\mGBAHttpServer.csproj -r $rid -p:AssemblyName="$($filenamePrefix)-$($rid)"
    #dotnet publish src\mGBAHttpServer\mGBAHttpServer.csproj -r $rid -p:AssemblyName="$($filenamePrefix)-$($rid)-self-contained" --self-contained=true -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true -p:DebuggerSupport=false

}

$win64SelfContainedFilename = "mGBA-http-{0}-win64-self-contained" -f $version
#dotnet publish .\mGBAHttpServer.csproj -c Release -f net8.0 -r win-x64 --self-contained true -p:PublishSingleFile=true -p:TrimMode=partial -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:JsonSerializerIsReflectionEnabledByDefault=true -p:DebuggerSupport=false -p:AssemblyName=$win64SelfContainedFilename

<#
$win64Filename = "mGBA-http-{0}-win64" -f $version
dotnet publish .\mGBAHttpServer.csproj -c Release -f net8.0 -r win-x64 --self-contained false  -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:DebuggerSupport=false -p:AssemblyName=$win64Filename
#>
