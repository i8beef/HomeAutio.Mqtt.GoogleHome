# Internal Architecture

The HomeAutio.Mqtt.GoogleHome application is a standalone .NET console application that exposes an OAuth server, WebAPI, MQTT client, and administrative UI. These primary components work in concert to bridge the Google Home SmartHome API fulfillment to the MQTT protocol where possible.

## Program and Startup

The `Program.cs` and `Startup.cs` files are the primary entry point and configuration location for the entire application. `Program.cs` is fairly standard, setting up configuration, logging, and creating the default `WebHost` that runs the application, and is fully configured in `Startup.cs`. The `Startup.cs` configures the entire application. `Startup.ConfigureServices` configures the dependency injection for the application, while `Startup.Configure` sets up the primary `WebHost` pipeline.

`ConfigureServices` is properly commented to differentiate different areas of dependency resolution. Particular components to pay attention to are:

1. MessageHub - Internal event pub/sub, used by various components to communicate internally without introducing direct synchronous dependencies.
2. GoogleDeviceRepository - Run time store of device configuration. Can be thought of as an abstraction over the `googleDevices.json` config persistence / config change publishing
3. StateCache - Run time state storage. Basically a Dictionary of MQTT topic to current value. Starts with an entry for all device state topics set to `string.Empty` until MQTT connection is made.
4. MQTT Service - MQTT background service which handles all MQTT communication.
5. GoogleHomeGraphService - Background service which handles `ReportState` and other events that must be published to Google Home Graph.
6. GoogleHomeGraphClient - Google Home Graph client which makes use of the downloaded `serviceAccountFile`. All interactions with this API go through this client.
7. Intent handlers - Google SmartHome intent handlers, one each for SYNC, QUERY, EXECUTE, and DISCONNECT (currently unused).
8. TokenCleanupService - OAuth background worker that handles cleaning out expired grants.
9. Proxy headers - Registers .NET rules for utilizing proxy forward headers.
10. IdentityServer4 - All configuration for the identity server.
11. Authorization - Adds .NET auth support for Cookies (for the UI) and JWT bearer tokens (for the API).

`Configure` sets up the primary WebHost pipeline by registering the middlewares that make it up. Order is important here, so special care should be taken that these are done in the right order.

### Middleware: RequestResponseLoggingMiddleware

A custom middleware called `RequestResponseLoggingMiddleware` is provided and registered for logging complete request / response pairs from requests made to the Google Home `/smarthome` route. This can be used to debug non-obvious issues with commands. These are automatically logged at the `Trace` level if verbose logging is enabled.

### X-Forward Headers

When proxied, the .NET runtime needs to be informed of this so that automatically generated URLs are created at the right location. The `Configure` method's `app.UseForwardedHeaders()` call enables this, and the headers are configured in `ConfigureServices` by the `ForwardedHeadersOptions` injection setup. The proxy needs to send through the X-Forwarded-Proto and X-Forwarded-For headers to make sure the app knows its being used on the HTTPS protocol specified by the X-Forwarded-Proto header, and its base host is translated to the one specified in the X-Forwarded-For header.

### App Base Path

The configuration exposes a `appPathBase` setting which should also be set to the relative URL portion of the public URL. For example, if hosted at `https://mySite.com/google/home`, it should be set to `/google/home`. This setting allows he .NET framework to generate URLs correctly instead of assuming the app base path is `/`.

## Configuration

Primary app configuration is housed in an app-root-relative `/config` directory. The app will ship with a default `appsettings.json` file in the app root that provides "default" values for all settings, and then the `config/appsettings.Environment.json` file overrides these defaults. It is recommended to simply copy this file and modify.

`Environment` is determined from the `ASPNETCORE_ENVIRONMENT` environment variable, or by default (or in debug), `Development`. In all Docker containers this will resolve to `Production`, thus ending up with a requiring a file called `config/appsettings.Production.json`.

The config file will specify a secondary config file location referred to as `googleDevices.json`, which will contain all device configuration. By default, this will be in `config/googleDevices.json` but can be changed if desired.

## Logging

Logging is performed via Serilog. By default, it will write all log messages (as configured) to both the Console and a designated log location (by default, a rolling file located at `logs/HomeAutio.Mqtt.GoogleHome.log`). Standard configurable log levels with `Information` as default are available. The HTTP server and the MQTT client will both output to this log file as well, and logs are sent "as is" from these libraries. As such, additional information can be output in Debug and Verbose mode that can be useful in identifying issues.

## OAuth Identity Server

The primary OAuth server is provided by [Identity Server 4](https://identityserver4.readthedocs.io/en/latest/). The identity server secures both the API endpoints, via authorization code flow, and UI access, via Cookie based authentication.

Configuration for the OAuth server is all under the `oauth` node of the configuration file, used as follows:

| Setting        | Usage |
| -------------- | ----- |
| tokenStoreFile | Location for simple persistent storage of OAuth tokens. Defaults to `config/tokens.json`. |
| authority      | IdentityServer authority URL. Should be set to the public URL Google would see as the app root, for example, `https://myPublicDomain.com/proxied/location`. Typical recommended to proxy at `/google/home`, which would become `https://myPublicDomain.com/google/home`, etc. |
| requireSSL     | Enables enforced protocol check in IdentityServer. As the app will be proxied and ALWAYS see basic HTTP, this should be left `false` |
| signingCerts   | Signing certificate information (path to `.pfx` and pass phrase). First cert is "active" while additional can be added to allow for temporary verification of signatures generated by expired certs to allow for cert rollover. |
| clients        | Client information used for bearer token auth. Usually contains a single client, the Google Actions client. |
| users          | User configuration. There is no separation of valid users for UI vs API here, so you can use a single user, or split off one for UI admin tasks and another for API usage (recommended just from a security perspective) |

### Cookie Auth

Cookie auth is used specifically for the UI. It establishes a regular sliding, 1 hour time frame for the cookie before requiring another authentication. It has to be set as the "default" auth method for the UI authentication to work, and the `Controllers` will not specify a preferred auth method because of this.

### Bearer Auth

Bearer auth is set up as a secondary supported auth method. The `GoogleHomeController` methods will be decorated with `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]` attributes to differentiate them as needing this auth scheme instead of the default Cookie auth.

### Token Cleanup Service

There is an additional background service called the `TokenCleanupService` which will examine all current token grants every 60 seconds. When tokens expire, they will remain in the token store until this service cleans them out. This is purely to keep the token store cleaned up.

### Refresh Token Grace Period

The identity server is fairly stringent on refresh token reuse. In the event that Google makes two concurrent requests with a refresh token, the first will *invalidate* the refresh token for further use by default. If the second call begins before the first has stored the new refresh token, then it will also try and refresh using that now invalid token, and fail, resulting in Google not having ANY valid refresh tokens.

To counteract this, there is a "graceful" mode that can be enabled in configuration with the `oauth:refreshTokenGracePeriod` setting for the number of seconds after first usage that a refresh token can still be used. By default, this is set to `0`, or disabled, but this option is available for this specific case. It is possible that this sohuld be defaulted to something like 3 seconds instead, which should be the maximum timeout used from the Google side of the call.

### Signing Certificates

Signing certificates can be generated with the following script which will last for 10 years:

```
#!/bin/bash

openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout signingKey.key -out signingKey.crt -subj "/CN=some.domain.com" -days 3650
openssl pkcs12 -export -out signingKey.pfx -inkey signingKey.key -in signingKey.crt
```

If signing certificate configuration information is omitted, the IdentityServer4 implementation will generate one on demand on first run at `config/tempkey.jwk`. These are for debug purposes *only* and should not be used in normal operation. They will expire quickly and are meant only for testing.

### Auth UI

IdentityServer4 provides a [default UI implementation](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI). There are scripts in the `/src` directory called `getmaster.ps1` and `getmaster.sh` depending on if you need to run them on Windows or Linux, which will update the "Quickstart" files from this repo. Note that the `AccountController` may require a minor change in the cosntructor to instantiate the `TestUserStore`. This is a possible location for improvement by setting this up for injection instead.

## Message Hub

Internal components communicate via an internal `MessageHub` in a basic pub/sub approach. Events published to this will be handed to subscribers to be processed, without direct dependency between components. Examples of usage include:

1. On EXECUTE intents, the `DeviceCommandExecutionEvent` is published internally for the MQTT service to translate and publish to MQTT.
2. On MQTT state changes, the `ReportStateEvent` is published by the `MqttService` for the `GoogleHomeGraphService` to handle for notifications to the Google Home Graph API.
3. Device configuration changes through the UI trigger `ConfigSubscriptionChangeEvent` which the `MqttService` wil llisten to to adjust its subscriptions for state topics on the fly.

Note that events can have multiple handlers.

## MQTT Service

The `MqttService` provides all interaction with MQTT. It is implemented as a background worker service, based on the `HomeAutio.Mqtt.Core` package. This package just provides some of the basic MQTT implementation, in turn based on the [MQTTNet](https://github.com/chkr1011/MQTTnet) library. Specifics of the MQTT background service are encapsulated in this package. Configuration is handled in the `appsettings.json` file in the `mqtt` section.

The service will, on startup and config change, automatically subscribe to any `state` topics from the `googleDevices.json` configuration. It will also subscribe to specific topics off the configured `topicRoot` (default `google/home/`). This allows for specific commands like `google/home/commands/REQUEST_SYNC/set` to trigger operations. At this time, this functionality is very limited as not many MQTT sourced commands really need to be supported.

On EXECUTE intents, the `MqttService` will receive a `DeviceCommandExecutionEvent` from the `MessageHub`. This is a *single device* command split out from the original intent. The params will be sent as is to `google/home/execution/deviceId/commandName` with a payload of the passed `params` from the intent, for external processing. In addition, it will attempt to map each param to a `googleDevices.json` state topic, and send the individual param values to the specified matched topic. This process can be hit or miss as command params are not one-to-one with state params.

There is a `CommandToStateKeyMapper` which handles this conversion for known key mismatches, but by its nature is a maintenance headache. This is an area that could use some better options in the future.

Other intents like SYNC and QUERY will also publish MQTT evets at `google/home/sync/lastRequest` and `google/home/query/lastRequest` as well.

## Google Home Fulfillment

All Google fulfillment is done via the `Controllers/GoogleHomeController`. It will route the incoming request to the various `IntentHandlers`, which will build the intent specific response to be returned. The controller handles building the primary response envelope, and delegates payload generation to the `IntentHandler`.

`DisconnectIntentHandler` is currently a no-op that simply logs the request was made.

`SyncIntentHandler` uses the current `GoogleDeviceRepository` to generate a SYNC response. As the Google device list has the same basic structure as a SYNC response, this is largely just a direct translation.

`QueryIntentHandler` uses the current `StateCache` to generate a result to a specific query intent. It also kicks off a Google Home Graph report state to ensure Home Graph gets an initial state on QUERY due to a Google oddity ([see this issue](https://github.com/actions-on-google/smart-home-nodejs/issues/224)).

`ExecuteIntentHandler` handles command executions. It will first evaluate "challenges" supported by Google Home for things like pin codes, etc.. It will split a single intent into individual devive commands and publish them to the internal `MessageHub` to be handled by the `MqttService`. It will also build a "dummy response" from the passed request by (a) getting the current QUERY response for the device and (b) overlaying passed param values, mapped to their state equivalent by the `CommandToStateKeyMapper`. In theory this gets *close* to a real response even though it is technically disconnected from "real" state. As MQTT is now a request/response protocol, there is not an easy alternative to block for the "real" state to update, which could bypass the need for the overlay.

## GoogleDeviceRepository and StateCache

The `GoogleDeviceRepository` is a real-time thread safe repository representation of the `googleDevices.json` configuration. It will be read and populated at startup, and will automatically persiste changes. Modification of items done via the `Add`, `Update` and `Delete` methods will automatically trigger `MessageHub` events to notify watchers, and automatically handle persistence. Various methods are exposed on items in the structure to facilitate combining with `StateCache` or getting different flattened or Google representations of the information.

The `StateCache` is essentially a thread safe ConcurrentDictionary of MQTT state topics to current state value (always a string, as MQTT payloads are not typed). These are automatically updated by the `MqttService` as they change.

## Administration Interface

The administrative user interface can be accessed by web browser at the root URL of the application. It allows for managing device configuration (under the hood, basically an editor for `GoogleDeviceRepository`). The UI is a simple MVC implementation using `Controllers`, `ViewModels` and `Views`. Much of the basic interface is just inherited from the IdentityServer4 Quickstart UI which also provides vertain views in the `Views` folder, which is in turn based on [Bootstrap 4](https://getbootstrap.com/). Two "themes" are provided, which were just taken from [Bootswatch](https://bootswatch.com).

Trait pieces are input as JSON blobs, with some basic formatting / tooling provided by [CodeMirror](https://codemirror.net/).

Changes are first passed through the validator before being accepted.

The main device list page also exposes some utility functions, like getting a complete "SYNC" resonse that can be handed to the Google SYNC validator to troubleshoot issues.

## Validation and Schema

Google supplies a [JSON schema repository](https://github.com/actions-on-google/smart-home-schema) for the smart home apis, which are faithfully downloaded and included in the `src/smart-home-schema` folder. These are mapped in the project to `/schema`, and are included as embedded resources on compile for use by the `TraitSchemaProvider`. The `TraitSchema` objects this provides allow for validation of JSON structures (see: validation of Attributes for traits in the UI), extracting embedded examples (see: UI trait examples), and other uses.

These schemas are built to validate individual request / response structures, so the Attribute schemas are the only piece that can be directly used for validation as is. Projecting these automatically into "HomeAutio.Mqtt.GoogleHome JSON Schemas" for validation is on the list for future expansion.

After updating the schema files by downloading and replacing the files in the schema directory, the script `src/generateDeviceFiles` can be run to regenerate `DeviceType`, `TraitType` and `CommandType` enums in the application automatically. This will ensure that all of these stay updated, and new devices and traits are automatically included just by updating the schema files and running this script.

# Docker

The application *can* be run in standalone mode, it is primarily distributed as a Docker container. Container files are found in `src/Dockerfile` and `src/Dockerfile.arm32`. The only special changes over standard ASP.NET Docker files involve setting environment variables for `ASPNETCORE_ENVIRONMENT` and `ASPNETCORE_URLS`. The application itself does not expose any option for exposing itself over HTTPS, so this is still religated to a proper proxy like nginx, etc., to handle those parts.

# Debugging and Contributing

Due to the proxy considerations, token management, etc., debugging in place is not easy. Repointing the proxy to a local debug instance is possible, but requires management of the token files, etc. to not get out of sync with the actual hosted implementation. It can be useful to debug normally and trigger endpoints with a tool like Postman to simulate Google calls instead. The SYNC response tool in the UI is also very helpful to find problematic device configurations without going this deep. `Trace` logging will also give a lot more information in logs when debugging issues, though its not recommended to leave it in this mode permanently as it can generate a lot of noise.

Contributions are welcome by PR. Be aware that .NET code analyzers will enforce various style and usage guidelines in VisualStudio, and will spit out compiler warnings that should be fixed before PR (Information only suggestions can be ignored). Unfortunately, the IdentityServer4 QuickStart files are also scanned by these, and because they are "external" code, those Warnings are ignored until a better option is available to exclude them from scanning. Compiler warnings can be filtered to not include the QuickStart files in the Error List UI in VisualStudio to find issues related to all other files.
