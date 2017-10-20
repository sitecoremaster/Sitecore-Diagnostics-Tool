# if code signing not required - remove this $CertificatePassword param and in PostCompile.ps1 all sections related to signing
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$CertificatePassword
)

$AppName = "Sitecore.DiagnosticsTool.WinApp.DEV"
$Title = "Sitecore Diagnostics Tool DEV (SDT)"
$BaseVersion = "1.2"
$Shift = -1096 ## +123 is initial offset that came from internal SVN during publishing to GitHub
$BaseApplicationURL = "http://dl.sitecore.net/updater/dev/sdt"
$PublisherName = "Sitecore Corporation"
$Icon = "Images\wizard.ico"
$SupportURL = "https://github.com/sitecore/sitecore-diagnostics-toolset/issues/new"
$CertificatePath = "C:\Sitecore\Keys\certificate.pfx"
$BuildOptions = "/p:PlatformTarget=x86"
$BuildFolder = [System.IO.Path]::GetFullPath("bin")
$OutputFolder = [System.IO.Path]::GetFullPath("publish")

Write-Host "> Build.ps1 -AppName `"$AppName`" -Title `"$Title`" -BaseVersion `"$BaseVersion`" -Shift `"$Shift`" -BuildFolder `"$BuildFolder`" -BuildOptions `"$BuildOptions`" -OutputFolder `"$OutputFolder`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""
Invoke-Expression ".\Build.ps1 -AppName `"$AppName`" -Title `"$Title`" -BaseVersion `"$BaseVersion`" -Shift `"$Shift`" -BuildFolder `"$BuildFolder`" -BuildOptions `"$BuildOptions`" -OutputFolder `"$OutputFolder`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""