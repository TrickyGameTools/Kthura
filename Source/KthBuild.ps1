function private:bld($packagedir,$packagename)
{
	$compiler = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe";

	write-host "Compiling: " -nonewline
	echo $packagename
    pushd
    cd $packagedir
	$result = &$compiler
	if (-Not $?){
	   popd
	   cls
	   write-host "ERROR COMPILING" -ForegroundColor Red
	   echo "Package: "+$packagename
	   echo $result
	   exit
	}
	popd
	
}

bld "KthuraLauncher" "Laucher"
