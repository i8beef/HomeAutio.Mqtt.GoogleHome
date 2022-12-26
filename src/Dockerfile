ARG VERSION=1.0.0.0
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
ARG VERSION
WORKDIR /app

# copy csproj and restore as distinct layers
COPY nuget.config .
COPY *.sln .
COPY HomeAutio.Mqtt.GoogleHome/*.csproj ./HomeAutio.Mqtt.GoogleHome/
COPY HomeAutio.Mqtt.GoogleHome.Tests/*.csproj ./HomeAutio.Mqtt.GoogleHome.Tests/
COPY IdentityServerHost.Quickstart.UI/*.csproj ./IdentityServerHost.Quickstart.UI/
RUN dotnet restore

# copy and build everything else
COPY smart-home-schema/. ./smart-home-schema/
COPY HomeAutio.Mqtt.GoogleHome/. ./HomeAutio.Mqtt.GoogleHome/
COPY HomeAutio.Mqtt.GoogleHome.Tests/. ./HomeAutio.Mqtt.GoogleHome.Tests/
COPY IdentityServerHost.Quickstart.UI/. ./IdentityServerHost.Quickstart.UI/
RUN dotnet build --configuration Release /p:Version=$VERSION

# publish
FROM build AS publish
WORKDIR /app/HomeAutio.Mqtt.GoogleHome
RUN dotnet publish -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS runtime
WORKDIR /app
COPY --from=publish /app/HomeAutio.Mqtt.GoogleHome/out ./

ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://+:5000

EXPOSE 5000/tcp

VOLUME ["/app/config", "/app/logs"]
ENTRYPOINT ["dotnet", "HomeAutio.Mqtt.GoogleHome.dll"]