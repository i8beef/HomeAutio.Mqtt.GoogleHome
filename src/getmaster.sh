#!/usr/bin/env bash

set -e

SOURCE="https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/main.zip"
curl -L -o ui.zip "$SOURCE"

unzip -d ui ui.zip

[[ -d HomeAutio.Mqtt.GoogleHome/Views ]] || mkdir HomeAutio.Mqtt.GoogleHome/Views
[[ -d HomeAutio.Mqtt.GoogleHome/wwwroot ]] || mkdir HomeAutio.Mqtt.GoogleHome/wwwroot

cp -r ./ui/IdentityServer4.Quickstart.UI-main/Quickstart/* IdentityServer4.Quickstart.UI
cp -r ./ui/IdentityServer4.Quickstart.UI-main/Views/* HomeAutio.Mqtt.GoogleHome/Views
cp -r ./ui/IdentityServer4.Quickstart.UI-main/wwwroot/* HomeAutio.Mqtt.GoogleHome/wwwroot

rm -rf ui ui.zip