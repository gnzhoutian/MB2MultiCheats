#!/bin/bash
# usage: ./make7z.sh 1.2.0/1.2.0-debug
set -x -e
TOP_PATH=$(cd `dirname $0`; pwd); cd $TOP_PATH
rm -rf tmpbuild; mkdir -p tmpbuild

VERSION=$1  # 1.2.0-debug
VER_MINI=$(echo $1 | awk -F'-' '{print $1}')  # 1.2.0


make_mod(){
    mod_name=$1
    mod_flag=$2

    cp -af src/_Module tmpbuild/${mod_name}
    cp -af src/_Module${mod_flag}/* tmpbuild/${mod_name}/

    pushd tmpbuild/${mod_name}
        sed -i 's!MOD_NAME!'${mod_name}'!g' SubModule.xml
        sed -i 's!MOD_VERSION!v'${VER_MINI}'!g' SubModule.xml

        mkdir -p bin/Win64_Shipping_Client
        cp -af ${TOP_PATH}/src/obj/Debug/MB2MultiCheats.dll bin/Win64_Shipping_Client/
        cp -af ${TOP_PATH}/src/obj/Debug/MB2MultiCheats.pdb bin/Win64_Shipping_Client/

        cp -af ${TOP_PATH}/LICENSE ./
        cp -af ${TOP_PATH}/README.md ./

        ${TOP_PATH}/tools/7za.exe a -r ../${mod_name}-${VERSION}.7z ./*
        mv -f ../${mod_name}-${VERSION}.7z ${TOP_PATH}/archives/
    popd

    # just for debug
    if [ -d "/d/Games/steam/steamapps/common/Mount & Blade II Bannerlord/Modules/${mod_name}" ]; then
        rm -rf "/d/Games/steam/steamapps/common/Mount & Blade II Bannerlord/Modules/${mod_name}"
        cp -af ${TOP_PATH}/tmpbuild/${mod_name} "/d/Games/steam/steamapps/common/Mount & Blade II Bannerlord/Modules/${mod_name}"
    fi 
}

# main
if [ "$1"x != ""x ];then
    make_mod MB2MultiCheats Origin
    make_mod MB2MultiCheatsSanGuo SanGuo
    make_mod MB2MultiCheatsShokuho Shokuho
else
    echo "入参错误，示例: ./make7z.sh 1.2.0/1.2.0-debug"
fi
