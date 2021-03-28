# Configuration

# Please note this is set to how my own personal system is configured.
# You may need to change this file if you have a different set up.
# Also note this file was meant for use in Visual Studio 2019

$sdl = "\Projects\Applications\VisualStudio\VC\3rdparty\SDL2-2.0.12"
$sdl_image = "E:\Projects\Applications\VisualStudio\VC\3rdparty\SDL2_image-2.0.5"



echo "Copying SDL2"
cp "$sdl\lib\x64\*.dll" "x64\Release"

echo "Copying SDL2_Image"
cp "$sdl_image\lib\x64\*.dll" "x64\Release"
