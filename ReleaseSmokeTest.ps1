<#
.SYNOPSIS
Smoke tests the published win-x64 release binary against a live mGBA.

.DESCRIPTION
Run this after ReleaseBuild.ps1. The script finds the win-x64 binary in .\release, starts it,
then sends one POST and one GET to each of the six endpoint groups. Two groups have no GET
endpoint, so they get only the POST.

The script prints one line per check, and a count at the end. It exits with code 1 if any check
failed. The binary writes its own output to a log file in the temporary folder.

mGBA must be running with mGBASocketServer.lua loaded and a ROM loaded.

The extension check loads BtnTest.gba, which replaces the ROM in mGBA, and the last check resets
the loaded ROM. Both run after every read-only check. A programmatic load also stops mGBA from
deriving save and savestate paths, so reload the ROM through the File menu before you run the
integration tests.
#>
param(
    [int]$Port = 5000,
    [int]$MgbaPort = 8888,
    [string]$RomPath = (Join-Path $PSScriptRoot "BtnTest.gba")
)

$ErrorActionPreference = "Stop"

$baseUrl = "http://127.0.0.1:$Port"
$outLog = Join-Path $env:TEMP "mGBA-http-smoke-out.log"
$errLog = Join-Path $env:TEMP "mGBA-http-smoke-err.log"

# 0x02000000, the start of EWRAM. The integration tests use this address as their scratch target.
$scratchAddress = 33554432

function Test-PortOpen {
    param(
        [int]$PortNumber,
        [int]$TimeoutMs = 500
    )

    $client = New-Object System.Net.Sockets.TcpClient
    try {
        $connect = $client.BeginConnect("127.0.0.1", $PortNumber, $null, $null)
        if (-not $connect.AsyncWaitHandle.WaitOne($TimeoutMs)) {
            return $false
        }
        $client.EndConnect($connect)
        return $true
    } catch {
        return $false
    } finally {
        $client.Close()
    }
}

function Invoke-Check {
    param(
        [string]$Method,
        [string]$Path,
        [string]$ExpectBody
    )

    $status = 0
    $body = ""
    $detail = ""

    try {
        $response = Invoke-WebRequest -Uri "$baseUrl$Path" -Method $Method -UseBasicParsing -TimeoutSec 15
        $status = [int]$response.StatusCode
        $body = [string]$response.Content
    } catch [System.Net.WebException] {
        if ($null -ne $_.Exception.Response) {
            $status = [int]$_.Exception.Response.StatusCode
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $body = $reader.ReadToEnd()
            $reader.Close()
        } else {
            $detail = $_.Exception.Message
        }
    } catch {
        $detail = $_.Exception.Message
    }

    $passed = ($status -ge 200 -and $status -lt 300)
    if ($passed -and $PSBoundParameters.ContainsKey("ExpectBody") -and $body -ne $ExpectBody) {
        $passed = $false
        $detail = "expected body '$ExpectBody'"
    }

    $shownPath = [System.Uri]::UnescapeDataString($Path)

    if ($passed) {
        Write-Host ("[PASS] {0,-4} {1}" -f $Method, $shownPath) -ForegroundColor Green
    } else {
        $reason = "-> $status"
        if ($detail -ne "") {
            $reason = "$reason $detail"
        } elseif ($body -ne "") {
            $shownBody = $body -replace "`r`n", " " -replace "`n", " "
            if ($shownBody.Length -gt 120) {
                $shownBody = $shownBody.Substring(0, 117) + "..."
            }
            $reason = "$reason $shownBody"
        }
        Write-Host ("[FAIL] {0,-4} {1} {2}" -f $Method, $shownPath, $reason) -ForegroundColor Red
    }

    return [pscustomobject]@{
        Method = $Method
        Path   = $shownPath
        Status = $status
        Body   = $body
        Detail = $detail
        Passed = $passed
    }
}

function Write-AppLog {
    foreach ($log in @($outLog, $errLog)) {
        if (-not (Test-Path $log)) {
            continue
        }
        $lines = @(Get-Content $log -Tail 20 -ErrorAction SilentlyContinue)
        if ($lines.Count -eq 0) {
            continue
        }
        Write-Host ""
        Write-Host "$log (last $($lines.Count) lines)" -ForegroundColor DarkGray
        foreach ($line in $lines) {
            # The app colours its output, and Windows PowerShell prints the escape codes literally.
            Write-Host ("  " + ($line -replace "\x1b\[[0-9;]*m", ""))
        }
    }
}

$releaseFolder = Join-Path $PSScriptRoot "release"
if (-not (Test-Path $releaseFolder)) {
    throw "No release folder at $releaseFolder. Run ReleaseBuild.ps1 first."
}

$binaries = @(Get-ChildItem -Path $releaseFolder -Filter "mGBA-http-*-win-x64.exe" -File)
if ($binaries.Count -eq 0) {
    throw "No win-x64 binary in $releaseFolder. Run ReleaseBuild.ps1 first."
}
if ($binaries.Count -gt 1) {
    throw "Found $($binaries.Count) win-x64 binaries in $releaseFolder. Delete the old ones: $($binaries.Name -join ', ')"
}
$binary = $binaries[0]

if (Test-PortOpen -PortNumber $Port) {
    throw "Port $Port is already in use. Stop the other mGBA-http instance, or pass -Port."
}
if (-not (Test-PortOpen -PortNumber $MgbaPort)) {
    throw "Nothing is listening on port $MgbaPort. Start mGBA, load mGBASocketServer.lua, then run this again."
}
if (-not (Test-Path $RomPath -PathType Leaf)) {
    throw "No ROM at $RomPath. The extension check needs one. Pass -RomPath to point at a ROM."
}

Remove-Item $outLog, $errLog -Force -ErrorAction SilentlyContinue

$process = Start-Process -FilePath $binary.FullName -ArgumentList "--urls", $baseUrl `
    -WorkingDirectory $releaseFolder -NoNewWindow -PassThru `
    -RedirectStandardOutput $outLog -RedirectStandardError $errLog

$checks = @()
$fatal = ""

try {
    $deadline = (Get-Date).AddSeconds(30)
    $ready = $false
    while ((Get-Date) -lt $deadline) {
        if ($process.HasExited) {
            throw "$($binary.Name) exited with code $($process.ExitCode) before it started listening."
        }
        if (Test-PortOpen -PortNumber $Port) {
            $ready = $true
            break
        }
        Start-Sleep -Milliseconds 250
    }
    if (-not $ready) {
        throw "$($binary.Name) did not start listening on port $Port within 30 seconds."
    }

    $checks += Invoke-Check -Method "GET" -Path "/scalar"

    $checks += Invoke-Check -Method "POST" -Path "/console/log?message=mGBA-http-release-smoke-test"

    $checks += Invoke-Check -Method "GET" -Path "/core/currentFrame"
    $checks += Invoke-Check -Method "POST" -Path "/core/clearkeys?keyBitmask=0"

    $checks += Invoke-Check -Method "GET" -Path "/mgba-http/button/getall"
    $checks += Invoke-Check -Method "POST" -Path "/mgba-http/button/tap?button=A"

    $read = Invoke-Check -Method "GET" -Path "/memorydomain/read8?memoryDomain=wram&address=$scratchAddress"
    $checks += $read
    if ($read.Passed) {
        # Write back the value just read, so the check leaves emulator memory unchanged.
        $checks += Invoke-Check -Method "POST" -Path "/memorydomain/write8?memoryDomain=wram&address=$scratchAddress&value=$($read.Body)"
    }

    $checks += Invoke-Check -Method "POST" -Path "/mgba-http/extension/loadfile?path=$([System.Uri]::EscapeDataString($RomPath))" -ExpectBody "true"

    $checks += Invoke-Check -Method "GET" -Path "/coreadapter/memory"
    $checks += Invoke-Check -Method "POST" -Path "/coreadapter/reset"
} catch {
    $fatal = $_.Exception.Message
} finally {
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        $process.WaitForExit(5000) | Out-Null
    }
}

if ($fatal -ne "") {
    Write-Host $fatal -ForegroundColor Red
    Write-AppLog
    exit 1
}

$failures = @($checks | Where-Object { -not $_.Passed })

if ($failures.Count -gt 0) {
    Write-Host ("{0} of {1} checks failed" -f $failures.Count, $checks.Count) -ForegroundColor Red
    Write-Host ("app log: {0}" -f $outLog) -ForegroundColor DarkGray
    exit 1
}

Write-Host ("{0} of {0} checks passed" -f $checks.Count) -ForegroundColor Green
exit 0
