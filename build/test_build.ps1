properties {
	$test_dir = "$source_dir\\Simple.Data.SqliteTests\bin\Release"
	$tests = @('Simple.Data.SqliteTests.dll') 
	$nunit = "$tool_dir\NUnit.2.5.10.11092\Tools\nunit-console-x86.exe"
}

task test -depends compile {

  if ($tests.Length -le 0) { 
     Write-Host -ForegroundColor Red 'No tests defined'
     return 
  }

  $test_assemblies = $tests | ForEach-Object { "$test_dir\$_" }

  & $nunit $test_assemblies /noshadow

    if($lastExitCode -ne 0) {
		throw "Tests Failed."
	}
	
	Write-Host "Finished Runnign Tests"
}