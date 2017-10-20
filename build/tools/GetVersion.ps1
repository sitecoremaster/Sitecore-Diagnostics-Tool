# params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [int]$Shift
)

# var
$CommitCountText = (git rev-list --count HEAD)
$CommitCount = [System.Int32]::Parse($CommitCountText)
$Revision = ($CommitCount + $Shift)

# do
$BaseVersion + ".0." + $Revision