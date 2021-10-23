![Build](https://github.com/i8beef/HomeAutio.Mqtt.GoogleHome/actions/workflows/build.yml/badge.svg?branch=master)
![Release](https://github.com/i8beef/HomeAutio.Mqtt.GoogleHome/actions/workflows/release.yml/badge.svg)

# HomeAutio.Mqtt.GoogleHome

This project is a bridge between Google Home's Actions API and MQTT. It allows for someone to implement a Google Home integration with their home automation project without needing a cloud service provider.

It provides an in process OAuth 2 server to satisfy the Google Actions Account Linking, and fulfillment for SYNC, QUERY, and EXECUTE intents. It then translates these messages back and forth to MQTT.

See the Wiki for more information about configuration and defining Google Device metadata.
