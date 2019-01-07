$source = "https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/master.zip"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest $source -OutFile ui.zip

Expand-Archive ui.zip

if (!(Test-Path -Path Quickstart))  { mkdir Quickstart }
if (!(Test-Path -Path Views))       { mkdir Views }
if (!(Test-Path -Path wwwroot))     { mkdir wwwroot }

copy .\ui\IdentityServer4.Quickstart.UI-master\Quickstart\* HomeAutio.Mqtt.GoogleHome\Quickstart -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-master\Views\* HomeAutio.Mqtt.GoogleHome\Views -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-master\wwwroot\* HomeAutio.Mqtt.GoogleHome\wwwroot -recurse -force

del ui.zip
del ui -recurse