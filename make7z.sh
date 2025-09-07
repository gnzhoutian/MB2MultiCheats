#!/bin/bash
# usage: make7z.sh 1.2.0
set -x -e
MOD_NAME=MultiCheats
TOP_PATH=$(cd `dirname $0`; pwd); cd $TOP_PATH

version=$1
sed -i 's!<Version value=.*$!<Version value="v'${version}'"/>!g' src/_Module/SubModule.xml

rm -rf tmpbuild; mkdir -p tmpbuild
cp -af src/_Module tmpbuild/${MOD_NAME}

pushd tmpbuild/${MOD_NAME}
	cp -af ${TOP_PATH}/pics ./
	cp -af ${TOP_PATH}/LICENSE ./
	cp -af ${TOP_PATH}/README.md ./
	
	${TOP_PATH}/tools/7za.exe a ../${MOD_NAME}-${version}.7z ./* -r
popd


# just for debug
cp -af ${TOP_PATH}/src/obj/Debug/*.dll ${TOP_PATH}/tmpbuild/${MOD_NAME}/bin/Win64_Shipping_Client/
cp -af ${TOP_PATH}/src/obj/Debug/*.pdb ${TOP_PATH}/tmpbuild/${MOD_NAME}/bin/Win64_Shipping_Client/
rm -rf "/d/Games/steam/steamapps/common/Mount & Blade II Bannerlord/Modules/${MOD_NAME}"
cp -af ${TOP_PATH}/tmpbuild/${MOD_NAME} "/d/Games/steam/steamapps/common/Mount & Blade II Bannerlord/Modules/${MOD_NAME}"
