# Configuration

# Please note this is set to how my own personal system is configured.
# You may need to change this file if you have a different set up.
# Also note this file was meant for use in Visual Studio 2019

# The project file for VS2019 has been configured to call this script
# automatically prior to compiling, however keep in mind that 
# PowerShell must be configured to allow script (which can for security
# reasons be turned off by default).

$sdl = "\Projects\Applications\VisualStudio\VC\3rdparty\SDL2-2.0.12"
$sdl_image = "E:\Projects\Applications\VisualStudio\VC\3rdparty\SDL2_image-2.0.5"
$zlib = "E:\projects\applications\visualstudio\vc\Apollo Game Engine\packages\zlib-msvc14-x64.1.2.11.7795\build\native\bin_release\" # Copied from my Apollo project


echo "Copying SDL2"
cp "$sdl\lib\x64\*.dll" "x64\Release"

echo "Copying SDL2_Image"
cp "$sdl_image\lib\x64\*.dll" "x64\Release"

echo "Copying zlib"
cp "$zlib\zlib.dll"
