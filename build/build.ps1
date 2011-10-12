properties {
	$version = if($env:BUILD_NUMBER) {$env:BUILD_NUMBER} else { "0.9.6.2" }
}

include .\master_build.ps1
include .\test_build.ps1
include .\package.ps1
task default -depends compile, test, package
