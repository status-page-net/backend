# Build

FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build

ARG NUGET_API_KEY
ARG NUGET_SOURCE
ARG SERVICE_REPOSITORY_URL
ARG SERVICE_VERSION

COPY ./ /opt/status-page-net/backend/
WORKDIR /opt/status-page-net/backend/

RUN \
	SERVICE_CONFIGURATION=Release \
	SERVICE_REPOSITORY_URL=${SERVICE_REPOSITORY_URL} \
	SERVICE_VERSION=${SERVICE_VERSION} \
	sh -x /opt/status-page-net/backend/build-dotnet.sh

RUN \
	NUGET_API_KEY=${NUGET_API_KEY} \
	NUGET_SOURCE=${NUGET_SOURCE} \
	sh -x /opt/status-page-net/backend/build-push.sh

# Runtime

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine

COPY --from=build /opt/status-page-net/backend/StatusPage.Server/out/Release/ /opt/status-page-net/backend/
WORKDIR /opt/status-page-net/backend/

ENTRYPOINT [ "dotnet", "StatusPage.Server.dll" ]