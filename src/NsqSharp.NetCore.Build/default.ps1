properties {
	$solutionDirectory = (Get-Item $solutionFile).DirectoryName
	$outputDirectory = "$solutionDirectory\.build"
	$tempOutputDirectory = "$outputDirectory\temp"

	$buildConfiguration = "Release"
	$buildPlatform = "Any CPU"
}

FormatTaskName "-------- {0} --------"

task default -depends Test

task Init `
	-requiredVariables outputDirectory, tempOutputDirectory `
{
	if (Test-Path $outputDirectory) {
		Write-Host "Removing previous artifacts in: $outputDirectory"
		Remove-Item $outputDirectory -Force -Recurse
	}

	Write-Host "Creating build directory at: $outputDirectory"
	New-Item $outputDirectory -ItemType Directory | Out-Null

	Write-Host "Creating temporary directory at: $tempOutputDirectory"
	New-Item $tempOutputDirectory -ItemType Directory | Out-Null
}

task Clean {
	Write-Host 'clean'
}

task Compile `
	-depends Init `
	-requiredVariables solutionFile, buildConfiguration, buildPlatform `
{
	Write-Host "Building solution: $solutionFile"
	Exec { msbuild $solutionFile "/p:Configuration=$buildConfiguration;Platform=$buildPlatform;OutDir=$tempOutputDirectory" }
}

task Test -depends  UnitTests {
}

task UnitTests -depends Compile, Clean {
	#Exec { dotnet test "..\NsqSharp.NetCore.UnitTests\NsqSharp.NetCore.UnitTests.csproj"}
}

task Nuget `
	-depends Compile `
	-requiredVariables buildConfiguration `
{
	Write-Host "Building nuget package"
	Exec { dotnet pack "..\NsqSharp.NetCore" --configuration=$buildConfiguration }
}

exit $LASTEXITCODE