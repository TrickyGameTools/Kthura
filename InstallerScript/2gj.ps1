$RELEASE="19.8.6-beta" 
$SCYNDI_SUPDATEGAMEJOLT_CONSTANT_TOKEN="7S5pJ0#*"
$GAMEID=328814
$INSTALLER_ID=420879
$NORM_ID=420878

echo "Game ID: $GAMEID"

echo "Release is $RELEASE --- Make sure that is ok!"
pause

pushd
cd ../Releases
zip ../InstallerScript/Kthura.zip * -9
popd

echo "gjpush -t=$SCYNDI_SUPDATEGAMEJOLT_CONSTANT_TOKEN -p=$INSTALLER_ID -r=$RELEASE -g=$GAMEID Kthura_Installer.zip"
echo "gjpush -t=$SCYNDI_SUPDATEGAMEJOLT_CONSTANT_TOKEN -p=$NORM_ID -r=$RELEASE -g=$GAMEID Kthura.zip"


gjpush "-t=$SCYNDI_SUPDATEGAMEJOLT_CONSTANT_TOKEN" "-p=$INSTALLER_ID" "-r=$RELEASE" "-g=$GAMEID" Kthura_Installer.zip
gjpush "-t=$SCYNDI_SUPDATEGAMEJOLT_CONSTANT_TOKEN" "-p=$NORM_ID" "-r=$RELEASE" "-g=$GAMEID" Kthura.zip
