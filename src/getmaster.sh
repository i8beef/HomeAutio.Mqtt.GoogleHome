#!/usr/bin/env bash

set -e

SOURCE="https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/master.zip"
curl -L -o ui.zip "$SOURCE"

unzip -d ui ui.zip

[[ -d HomeAutio.Mqtt.GoogleHome/Quickstart ]] || mkdir HomeAutio.Mqtt.GoogleHome/Quickstart
[[ -d HomeAutio.Mqtt.GoogleHome/Views ]] || mkdir HomeAutio.Mqtt.GoogleHome/Views
[[ -d HomeAutio.Mqtt.GoogleHome/wwwroot ]] || mkdir HomeAutio.Mqtt.GoogleHome/wwwroot

cp -r ./ui/IdentityServer4.Quickstart.UI-master/Quickstart/* HomeAutio.Mqtt.GoogleHome/Quickstart
cp -r ./ui/IdentityServer4.Quickstart.UI-master/Views/* HomeAutio.Mqtt.GoogleHome/Views
cp -r ./ui/IdentityServer4.Quickstart.UI-master/wwwroot/* HomeAutio.Mqtt.GoogleHome/wwwroot

rm -rf ui ui.zip