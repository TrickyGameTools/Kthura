echo "Synchronzing assets"
pushd
cd ..
kthbuild
popd
cp "\Projects\Applications\VisualStudio\Kthura\Releases\*.jcr" "x64/Release"
