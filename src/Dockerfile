FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY nuget.config .
COPY *.sln .
COPY HomeAutio.Mqtt.GoogleHome/*.csproj ./HomeAutio.Mqtt.GoogleHome/
RUN dotnet restore

# copy and build everything else
COPY HomeAutio.Mqtt.GoogleHome/. ./HomeAutio.Mqtt.GoogleHome/
RUN dotnet build -c Release

# publish
FROM build AS publish
WORKDIR /app/HomeAutio.Mqtt.GoogleHome
RUN dotnet publish -o out

# build runtime image
FROM microsoft/dotnet:2.1-runtime-alpine AS runtime
WORKDIR /app
COPY --from=publish /app/HomeAutio.Mqtt.GoogleHome/out ./

EXPOSE 5000
VOLUME ["/app/appsettings.json", "/app/googleDevices.json", "/app/tempkey.rsa", "/app/tokens.json", "/app/logs"]
ENTRYPOINT ["dotnet", "HomeAutio.Mqtt.GoogleHome.dll"]