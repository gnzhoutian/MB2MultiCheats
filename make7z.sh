#!/bin/bash
# usage: make7z.sh 1.2.0
set -x -e
TOP_PATH=$(cd `dirname $0`; pwd) ;cd $TOP_PATH
TMP_PATH=${TOP_PATH}/../tmpbuild
EXE7Z=${TOP_PATH}/tools/7za.exe

version=$1

sed -i 's!<Version value=.*$!<Version value="v'${version}'"/>!g' SubModule.xml

pushd ${TMP_PATH}
    rm -rf MultiCheats
    mkdir -p MultiCheats
popd

pushd ${TMP_PATH}/MultiCheats
    cp -af ${TOP_PATH}/bin ./
    cp -af ${TOP_PATH}/ModuleData ./
    cp -af ${TOP_PATH}/LICENSE ./
    cp -af ${TOP_PATH}/README.md ./
    cp -af ${TOP_PATH}/SubModule.xml ./
    cp -af ${TOP_PATH}/src ./
    
    rm -rf src/.vs src/bin src/obj
    rm -f src/MultiCheats.csproj.user
    
    ${EXE7Z} a ../MultiCheats-${version}.7z ./* -r
popd
