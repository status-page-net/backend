set -e
set -u

dotnet restore --configfile NuGet.Config
cd azure-functions-host
dotnet build --configuration Release --no-restore /p:Version=${SERVICE_VERSION} /p:RepositoryUrl=${SERVICE_REPOSITORY_URL} src/WebJobs.Script.WebHost/WebJobs.Script.WebHost.csproj
cd ..
dotnet build --configuration Release --no-restore /p:Version=${SERVICE_VERSION} /p:RepositoryUrl=${SERVICE_REPOSITORY_URL}
dotnet test --configuration Release --no-build --verbosity normal

dotnet nuget push **/*.nupkg --api-key ${NUGET_API_KEY} --source ${NUGET_SOURCE} --no-symbols true