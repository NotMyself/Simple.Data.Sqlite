properties {
	$nuget_pack_dir = "$build_dir\nuget_pub\"
	$nuget = "$tool_dir\nuget\NuGet.exe"
}

task package -depends test {
	New-Item $nuget_pack_dir\lib\net40 -itemType directory | Out-Null
	Copy-Item "$build_output_dir\Simple.Data.Sqlite.dll" $nuget_pack_dir\lib\net40
	Copy-Item "$base_dir\build\Simple.Data.Sqlite.nuspec" $nuget_pack_dir
	& $nuget pack "$base_dir\build\Simple.Data.Sqlite.nuspec" #/o "$build_dir\nuget"
}


