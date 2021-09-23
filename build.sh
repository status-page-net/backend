set -e
set -u

# docker build

# Separate quiet pull for clean build logs.
docker pull --quiet mcr.microsoft.com/dotnet/aspnet:5.0-alpine
docker pull --quiet mcr.microsoft.com/dotnet/sdk:5.0-alpine

docker build \
	--network=host \
	--build-arg NUGET_API_KEY=${NUGET_API_KEY} \
	--build-arg NUGET_SOURCE=${NUGET_SOURCE} \
	--build-arg SERVICE_REPOSITORY_URL=${SERVICE_REPOSITORY_URL} \
	--build-arg SERVICE_VERSION=${SERVICE_VERSION} \
	-t ${SERVICE_IMAGE_NAME}:${SERVICE_VERSION} \
	.
