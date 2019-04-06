# What is vital to understand is that Kthura is a multi-app package
# Just using any macros or constants containing build dates wouldn't do
# as that only captures that specific app. This routine is meant to 
# capture, the last build date of Kthura as a whole!
function private:builddate{
   $date = Get-Date -UFormat "%A, %B %e, %Y %t %r"
   $csclass = 'namespace Kthura { static class BuildDate { readonly static public string sBuildDate = "'
   $csclass = $csclass + $date
   $csclass = $csclass + '"; }} // This file is generated by the building script. '
   echo $csclass > "KthuraLauncher/builddate.cs"
   write-host "Build date: " -nonewline -foregroundcolor yellow
   write-host $date -foregroundcolor cyan
}

function private:bld($packagedir,$packagename) {
	$compiler = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe";

	write-host "Compiling: " -nonewline -Foregroundcolor Yellow
	write-host $packagename -ForeGroundColor Cyan
    pushd
    cd $packagedir
	$result = &$compiler
	if (-Not $?){
	   popd
	   #cls
	   write-host "ERROR COMPILING" -ForegroundColor Red
	   echo "Package: "+$packagename
	   echo $result
	   exit
	}
	popd
	
}

function private:cpy($bindir,$packagename){
	write-host "  Copying: " -nonewline -Foregroundcolor Yellow
	write-host $packagename -ForeGroundColor Cyan
	copy-item $bindir ../Releases
}

function private:clearreleases{
	write-host "Clearing Release Folder " -Foreground Yellow
    $Readme=cat ../Releases/ReadMe.md
    del ../Releases/*.*
    echo $Readme > ../Releases/ReadMe.md
}

function private:PackAssets{
	write-host "  Packing: " -nonewline -Foregroundcolor Yellow
	write-host "Assets" -ForeGroundColor Cyan
    $result = jcr6 add -i ../Assets ../Releases/KthuraEdit.jcr
    if (-Not $?) {
       echo $result
       write-host "ERROR PACKING" -ForegroundColor Red
       exit
    }
}

builddate
clearreleases
PackAssets
bld "KthuraLauncher" "Launcher"
cpy "Kthuralauncher/bin/Release/*.exe" "Launcher"
bld "KthuraEdit" "Editor"
cpy "KthuraEdit/bin/Windows/x86/Release/*.exe" "Editor Executables"
cpy "KthuraEdit/bin/Windows/x86/Release/*.dll" "Editor Libraries"
cpy "KthuraEdit/bin/Windows/x86/Release/*.xml" "Editor Data"
