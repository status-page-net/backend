set -e
set -u

dotnet nuget push **/*.nupkg --api-key ${NUGET_API_KEY} --source ${NUGET_SOURCE} --no-symbols true