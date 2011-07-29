properties {
	$nuget_pub_dir = "$base_dir\releases\"
	$nuget = "$tool_dir\nuget\NuGet.exe"
}

task package -depends test {
	Push-Location "$source_dir\Simple.Data.Sqlite"
	& $nuget pack .\Simple.Data.Sqlite.csproj -Sym -Properties Configuration=Release -Version $version -OutputDirectory $nuget_pub_dir
	Pop-Location
}

task publish {
	Push-Location "$nuget_pub_dir"
	#& $nuget push
	ls *$version.nupkg | ForEach-Object { 
			& $nuget push $_ 
		}
	Pop-Location
}


