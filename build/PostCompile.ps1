# params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$Name,
    [Parameter(Mandatory=$True)] [string]$AppName,
    [Parameter(Mandatory=$True)] [string]$Title,
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [string]$Shift,
    [Parameter(Mandatory=$True)] [string]$BuildFolder,
    [Parameter(Mandatory=$True)] [string]$OutputFolder,
    [Parameter(Mandatory=$True)] [string]$Icon,
    [Parameter(Mandatory=$True)] [string]$BaseApplicationURL,
    [Parameter(Mandatory=$True)] [string]$SupportURL,
    [Parameter(Mandatory=$True)] [string]$PublisherName,
    [Parameter(Mandatory=$True)] [string]$CertificatePath,
    [Parameter(Mandatory=$True)] [string]$CertificatePassword
)

# get version
Write-Host "tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`" -Shift `"$Shift`""
$Version = Invoke-Expression ".\tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`" -Shift `"$Shift`""

# normalize paths
$OutputFolder = [System.IO.Path]::GetFullPath($OutputFolder)
$BuildFolder = [System.IO.Path]::GetFullPath($BuildFolder)

# cleanup useless files
DIR "$BuildFolder" -Filter *.pdb -Recurse | Remove-Item
DIR "$BuildFolder" -Filter *.xml -Recurse | Remove-Item 
DIR "$BuildFolder" -Filter *.config -Recurse | Remove-Item 

# recreate $OutputFolder 
New-Item $OutputFolder -ItemType Directory -Force
Remove-Item $OutputFolder -Recurse -Force 
New-Item $OutputFolder -ItemType Directory

Write-Host "> Done. Recreated [$OutputFolder] folder"
Write-Host ""

# vars
$ReleaseFolder = "$OutputFolder\Application Files\$Name.$Version"
$ApplicationFileName = "$AppName.application"
$ExecutableFile = "$ReleaseFolder\$Name.exe"
$ApplicationFile = "$OutputFolder\$ApplicationFileName"
$URL = "$BaseApplicationUrl/$ApplicationFileName"
$ManifestFile = "$ExecutableFile.manifest"

# create parent folder of $ReleaseFolder
MKDIR $ReleaseFolder
RMDIR $ReleaseFolder -Force

# move
Write-Host "Move-Item $BuildFolder -Destination $ReleaseFolder"
Move-Item $BuildFolder -Destination $ReleaseFolder

# clean up files
"" | Set-Content "stderr.txt"
"" | Set-Content "stdout.txt"

# sign executable file
Write-Host "> tools\signtool.exe sign /f `"$CertificatePath`" /p `"$CertificatePassword`" /t `"http://timestamp.verisign.com/scripts/timstamp.dll`" `"$ExecutableFile`""
Start-Process -FilePath "tools\signtool.exe" -ArgumentList "sign /f `"$CertificatePath`" /p `"$CertificatePassword`" /t `"http://timestamp.verisign.com/scripts/timstamp.dll`" `"$ExecutableFile`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# create manifest
Write-Host "> tools\mage.exe -New Application -ToFile `"$ManifestFile`" -FromDirectory `"$ReleaseFolder`" -Name `"$Name`" -Version `"$Version`" -IconFile `"$Icon`" -Processor x86"
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-New Application -ToFile `"$ManifestFile`" -FromDirectory `"$ReleaseFolder`" -Name `"$Name`" -Version `"$Version`" -IconFile `"$Icon`" -Processor x86" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# sign manifest
Write-Host "> tools\mage.exe -Sign `"$ManifestFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`""
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-Sign `"$ManifestFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# create application file
Write-Host "> tools\mage.exe -New Deployment -AppManifest `"$ManifestFile`" -ToFile `"$ApplicationFile`" -Processor x86 -Install true -Publisher `"$PublisherName`" -ProviderUrl `"$URL`""
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-New Deployment -AppManifest `"$ManifestFile`" -ToFile `"$ApplicationFile`" -Processor x86 -Install true -Publisher `"$PublisherName`" -ProviderUrl `"$URL`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# workaround to replace some wrong values that I have no idea they came from and why + add a couple things mage doesn't seem to support (SupportUrl)
(
    (
            (                                        
                (
                    Get-Content $ApplicationFile
                ) -replace "asmv2:product=`"[^`"]+`"", "asmv2:product=`"$Title`" asmv2:supportUrl=`"$SupportUrl`""
            ) -replace "name=`"[^`"]+.app`" version=`"1.0.0.0`"", "name=`"$ApplicationFileName`" version=`"$Version`""
    ) -replace "<framework targetVersion=`"\d+.\d+`" profile=`"Client`" .+", ""
) | Set-Content $ApplicationFile

# sign application file
Write-Host "> tools\mage.exe -Sign `"$ApplicationFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`""
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-Sign `"$ApplicationFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host
