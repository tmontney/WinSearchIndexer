function Invoke-PostSearchIndexCommandResult([bool]$Result, [string]$MessageOnTrue, [string]$MessageOnFalse) {
    if ($Result) {
        Write-Output $MessageOnTrue
    }
    else {
        Write-Error -Message $MessageOnFalse
    }
}

$ErrorActionPreference = 'Stop'

Import-Module ".\lib\Microsoft.Search.Interop.dll"
Import-Module ".\lib\WinSearchIndexer.dll"

$TestPath = "C:\Test"
$ExcludedTestPath = "C:\Test\DontPickMe"

# Make sure we have our test folders created
if (-not (Test-Path -Path $TestPath)) { Write-Error -Message "The path '$TestPath' must exist for this test" }
if (-not (Test-Path -Path $ExcludedTestPath)) { Write-Error -Message "The path '$ExcludedTestPath' must exist for this test" }

# Index our entire test folder and all children
# Specify -OverrideChildRules to start fresh
Add-SearchIndexIncludedLocation -FilePath "$TestPath\*" -OverrideChildRules $true
# Confirm it worked (if something really bad happened, an exception would've been thrown)
Invoke-PostSearchIndexCommandResult -Result (Find-SearchIndexLocationIncluded -FilePath "$TestPath\*") `
    -MessageOnTrue "$TestPath and all children are indexed" -MessageOnFalse "Failed to index $TestPath"

# Now exclude a sub-folder
Remove-SearchIndexIncludedLocation -FilePath "$ExcludedTestPath\*"
# Confirm it's NOT included
Invoke-PostSearchIndexCommandResult -Result (-not (Find-SearchIndexLocationIncluded -FilePath "$ExcludedTestPath\*")) `
    -MessageOnTrue "$ExcludedTestPath has been excluded from indexing" `
    -MessageOnFalse "$ExcludedTestPath appears to still be included for indexing, despite excluding it"

# Finally, confirm -OverrideChildRules restores its parent's indexing state
Add-SearchIndexIncludedLocation -FilePath "$TestPath\*" -OverrideChildRules $true
# Confirm that it IS included again
Invoke-PostSearchIndexCommandResult -Result (Find-SearchIndexLocationIncluded -FilePath "$ExcludedTestPath\*") `
    -MessageOnTrue "Re-applied indexing to $TestPath and all children; $ExcludedTestPath is no longer excluded" `
    -MessageOnFalse "Re-applied indexing to $TestPath and all children; however $ExcludedTestPath appears to still be excluded"

# You will probably still need to exit this session, if you want to free the lock on the DLL
Remove-Module "WinSearchIndexer"
Remove-Module "Microsoft.Search.Interop"