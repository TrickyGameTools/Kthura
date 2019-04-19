# This script will compile all appliget-contentions, and build up the assets 
# file. Before doing so, you need to check a few things:
#
# This script requires the JCR6 cli tools, and they must be in the 
# system's path. Make sure you installed those, and that their dir is
# in the path.
#
# In the "bld" function you can see my path to msbuild.exe 
# That's what it was on my system. Is it the same in yours? If not
# edit the $compiler variable there!
#
# Next make sure that Kthura's folder lives in a parent folder totally
# dediget-contented to my projects alone. Make sure that from there TQMG,
# TrickyUnits and JCR6 are also available.
#
# If any of those things are not well-set, this script won't work!





# What is vital to understand is that Kthura is a multi-app package
# Just using any macros or constants containing build dates wouldn't do
# as that only captures that specific app. This routine is meant to 
# capture, the last build date of Kthura as a whole!
function private:builddate{
   $date = Get-Date -UFormat "%A, %B %e, %Y %t %r"
   $csclass = 'static class BuildDate { readonly static public string sBuildDate = "'
   $csclass = $csclass + $date
   $csclass = $csclass + '"; } // This file is generated by the building script. '
   Write-Output $csclass > "KthuraLauncher/builddate.cs"
   write-host "Build date: " -nonewline -foregroundcolor yellow
   write-host $date -foregroundcolor cyan
}

function private:bld($packagedir,$packagename) {
	$compiler = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe";
	$result = "---"
	write-host "Compiling: " -nonewline -Foregroundcolor Yellow
	write-host $packagename -ForeGroundColor Cyan
    Push-Location
	Set-Location $packagedir
	if ($packagedir -eq "KthuraTextEditor"){
		$result = &$compiler KthuraTextEditorImpInKthura.sln /p:Configuration=Release	
	} else {
		$result = &$compiler /p:Configuration=Release
	}
	if (-Not $?){
	   Pop-location
	   #cls
	   write-host "ERROR COMPILING" -ForegroundColor Red
	   write-output "Package: "+$packagename
	   write-output $result
	   exit
	}
	Pop-Location
	
}

function private:cpy($bindir,$packagename){
	write-host "  Copying: " -nonewline -Foregroundcolor Yellow
	write-host $packagename -ForeGroundColor Cyan
	copy-item $bindir ../Releases
}

function private:clearreleases{
	write-host "Clearing Release Folder " -Foreground Yellow
    $Readme=get-content ../Releases/ReadMe.md
    remove-item ../Releases/*.*
    write-output $Readme > ../Releases/ReadMe.md
}

function private:PackAssets{
	write-host "  Packing: " -nonewline -Foregroundcolor Yellow
	write-host "Assets" -ForeGroundColor Cyan
    $result = jcr6 add -i ../Assets ../Releases/KthuraEdit.jcr
    if (-Not $?) {
       write-output $result
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
if (Test-Path "KthuraTextEditor"){  #This "if" is needed since the TextEditor is optional!
	bld "KthuraTextEditor" "Text Editor"
	cpy "KthuraTextEditor/bin/Release/*.exe" "Text Editor Executables"
	cpy "KthuraTextEditor/bin/Release/*.dll" "Text Editor Libraries"
}
