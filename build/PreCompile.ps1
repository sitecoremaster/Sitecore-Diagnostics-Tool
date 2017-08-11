# get params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [string]$Shift,
    [Parameter(Mandatory=$True)] [string]$BuildFolder,
    [Parameter(Mandatory=$True)] [string]$ProjectFile
)

# normalize paths
$ProjectFile =  [System.IO.Path]::GetFullPath($ProjectFile)
$OutputFolder = [System.IO.Path]::GetFullPath($OutputFolder)
$BuildFolder = [System.IO.Path]::GetFullPath($BuildFolder)

# get version
Write-Host "tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`" -Shift `"$Shift`""
$Version = Invoke-Expression ".\tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`" -Shift `"$Shift`""

# delete build folder
New-Item $BuildFolder -ItemType Directory -Force
Remove-Item $BuildFolder -Recurse -Force 

Write-Host "> Done. Deleted [$BuildFolder] folder"
Write-Host ""

# update assemblyinfo.cs
DIR ..\src AssemblyInfo.cs -Recurse | 
  ForEach-Object { $_.FullName } | 
  ForEach-Object { 
    Write-Host ".\tools\UpdateAssemblyInfo.exe --path $_ --version $Version --force false"
    (.\tools\UpdateAssemblyInfo.exe --path $_ --version $Version --force false )
  } 

Write-Host "> Done. Updated assembly info"