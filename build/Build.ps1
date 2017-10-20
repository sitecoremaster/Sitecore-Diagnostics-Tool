# get params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$AppName,
    [Parameter(Mandatory=$True)] [string]$Title,
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [string]$Shift,
    [Parameter(Mandatory=$True)] [string]$PublisherName,
    [Parameter(Mandatory=$True)] [string]$Icon,
    [Parameter(Mandatory=$True)] [string]$BaseApplicationUrl,
    [Parameter(Mandatory=$True)] [string]$SupportURL,
    [Parameter(Mandatory=$True)] [string]$CertificatePath,
# if code signing not required - remove this $CertificatePassword param and in PostCompile.ps1 all sections related to signing
    [Parameter(Mandatory=$True)] [string]$CertificatePassword,
    [Parameter(Mandatory=$True)] [string]$BuildFolder,
    [Parameter(Mandatory=$True)] [string]$BuildOptions,
    [Parameter(Mandatory=$True)] [string]$OutputFolder
)

# vars
$Name = "Sitecore.DiagnosticsTool.WinApp"
$ProjectFile = [System.IO.Path]::GetFullPath("..\src\Sitecore.DiagnosticsTool.WinApp\SDT.WinApp.csproj")

Write-Host "> PreCompile.ps1 -Shift `"$Shift`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -ProjectFile `"$ProjectFile`""
Invoke-Expression ".\PreCompile.ps1 -Shift `"$Shift`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -ProjectFile `"$ProjectFile`""

Write-Host "> DoCompile.ps1 -BuildFolder `"$BuildFolder`" -BuildOptions `"$BuildOptions`" -ProjectFile `"$ProjectFile`""
Invoke-Expression ".\DoCompile.ps1 -BuildFolder `"$BuildFolder`" -BuildOptions `"$BuildOptions`" -ProjectFile `"$ProjectFile`""

Write-Host "> PostCompile.ps1 -Name `"$Name`" -AppName `"$AppName`" -Title `"$Title`" -Shift `"$Shift`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -OutputFolder `"$OutputFolder`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""
Invoke-Expression ".\PostCompile.ps1 -Name `"$Name`" -AppName `"$AppName`" -Title `"$Title`" -Shift `"$Shift`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -OutputFolder `"$OutputFolder`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""

"" | Set-Content "stderr.txt"
Remove-Item "stderr.txt"

"" | Set-Content "stdout.txt"
Remove-Item "stdout.txt"

