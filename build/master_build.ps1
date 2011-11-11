properties {
	$base_dir = resolve-path ..\
	$source_dir = "$base_dir\src\"
	$tool_dir = "$base_dir\packages\"
	$sharedAssemblyInfo = "$base_dir\src\ProjectInfo.cs"
	$sln_file = "$base_dir\Solution.sln"
	$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
}

task version {
	$version_pattern = "\d*\.\d*\.\d*\.\d*"
	$content = Get-Content $sharedAssemblyInfo `
		| ForEach { [regex]::replace($_, $version_pattern, $version) } 

	Set-Content -Value $content -Path $sharedAssemblyInfo
}

task compile -depends version {

	& $msbuild $sln_file /p:Configuration=Release
  
  if($lastExitCode -ne 0) {
		throw "Compile Failed."
	}
}