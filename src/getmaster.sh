#!/usr/bin/env bash

set -e

SOURCE="https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/master.zip"
curl -L -o ui.zip "$SOURCE"

unzip -d ui ui.zip

[[ -d Quickstart ]] || mkdir Quickstart
[[ -d Views ]] || mkdir Views
[[ -d wwwroot ]] || mkdir wwwroot

cp -r ./ui/IdentityServer4.Quickstart.UI-master/Quickstart/* HomeAutio.Mqtt.GoogleHome/Quickstart
cp -r ./ui/IdentityServer4.Quickstart.UI-master/Views/* HomeAutio.Mqtt.GoogleHome/Views
cp -r ./ui/IdentityServer4.Quickstart.UI-master/wwwroot/* HomeAutio.Mqtt.GoogleHome/wwwroot

rm -rf ui ui.zip