set -e
set -u

dotnet restore --disable-parallel
dotnet build --configuration ${SERVICE_CONFIGURATION} --no-restore /p:Version=${SERVICE_VERSION} /p:RepositoryUrl=${SERVICE_REPOSITORY_URL}
dotnet test --configuration ${SERVICE_CONFIGURATION} --no-build --verbosity normal

SERVER_PROJECT=StatusPage.Server
SERVER_PATH=${SERVER_PROJECT}/out/${SERVICE_CONFIGURATION}

dotnet publish --configuration ${SERVICE_CONFIGURATION} --no-build --output ${SERVER_PATH} ${SERVER_PROJECT}/${SERVER_PROJECT}.csproj