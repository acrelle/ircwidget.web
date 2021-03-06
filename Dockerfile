FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY IrcWidget.Web/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY IrcWidget.Web/ ./
RUN dotnet publish -c Release -o out && \
    mkdir /logs

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
LABEL maintainer=anthony@relle.co.uk

WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "IrcWidget.Web.dll"]

ENV LogFile=/logs/logfile
