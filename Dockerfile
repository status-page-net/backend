# Build

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

ARG NUGET_API_KEY
ARG NUGET_SOURCE
ARG SERVICE_REPOSITORY_URL
ARG SERVICE_VERSION

COPY ./ /opt/status-page-net/backend/
WORKDIR /opt/status-page-net/backend/

RUN \
	NUGET_API_KEY=${NUGET_API_KEY} \
	NUGET_SOURCE=${NUGET_SOURCE} \
	SERVICE_REPOSITORY_URL=${SERVICE_REPOSITORY_URL} \
	SERVICE_VERSION=${SERVICE_VERSION} \
	sh -x /opt/status-page-net/backend/build-dotnet.sh
RUN dotnet publish --configuration Release --no-build --output out StatusPage.Function

# Runtime

FROM mcr.microsoft.com/dotnet/aspnet:3.1

COPY --from=build /opt/status-page-net/backend/out/ /opt/status-page-net/backend/
WORKDIR /opt/status-page-net/backend/
