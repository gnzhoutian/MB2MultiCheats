#!/bin/bash
# usage: ./make7z.sh all/Origin/SanGuo 1.3.0
set -x -e
TOP_PATH=$(cd `dirname $0`; pwd); cd $TOP_PATH
rm -rf tmpbuild; mkdir -p tmpbuild

MOD_TYPE=$1  # all/Origin/SanGuo
MOD_VERSION=$2  # 1.3.0

make_mod(){
    mod_name=$1
    mod_flag=$2
    sed -i 's!<Version value=.*$!<Version value="v'${MOD_VERSION}'"/>!g' src/_Module${mod_flag}/SubModule.xml

    cp -af src/_Module${mod_flag} tmpbuild/${mod_name}

    pushd tmpbuild/${mod_name}
        cp -af ${TOP_PATH}/src/_Module/* ./
        cp -af ${TOP_PATH}/pics ./
        cp -af ${TOP_PATH}/LICENSE ./
        cp -af ${TOP_PATH}/README.md ./

        ${TOP_PATH}/tools/7za.exe a -r ../${mod_name}-${MOD_VERSION}.7z ./*
        cp -af ../${mod_name}-${MOD_VERSION}.7z ${TOP_PATH}/archives/
    popd

    # just for debug
    cp -af ${TOP_PATH}/src/obj/Debug/*.dll ${TOP_PATH}/tmpbuild/${mod_name}/bin/Win64_Shipping_Client/
    cp -af ${TOP_PATH}/src/obj/Debug/*.pdb ${TOP_PATH}/tmpbuild/${mod_name}/bin/Win64_Shipping_Client/
    rm -rf "/d/Games/steam/steamapps/common/Mount & Blade II Bannerlord/Modules/${mod_name}"
    cp -af ${TOP_PATH}/tmpbuild/${mod_name} "/d/Games/steam/steamapps/common/Mount & Blade II Bannerlord/Modules/${mod_name}"
}

# main
if [ "$2"x != ""x ];then
    case $MOD_TYPE in
    "all")
        make_mod MB2MultiCheats Origin
        make_mod MB2MultiCheatsSanGuo SanGuo
        ;;
    "Origin")
        make_mod MB2MultiCheats Origin
        ;;
    "SanGuo")
        make_mod MB2MultiCheatsSanGuo SanGuo
        ;;
    *)
        echo "入参错误，示例: ./make7z.sh all 1.3.0"
        ;;
    esac
else
    echo "入参错误，示例: ./make7z.sh all 1.3.0"
fi
