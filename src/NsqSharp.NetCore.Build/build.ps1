cls

Remove-Module [p]sake

# Get psake module path
$psakeModule = (Get-ChildItem (".\psake-*\psake.psm1")).FullName | Sort-Object $_ | select -Last 1

Import-Module $psakeModule
Invoke-Psake -buildFile .\default.ps1 `
	-taskList Test `
	-parameters @{ "solutionFile" = "..\NsqSharp.sln" }