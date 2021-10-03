set -e
set -u

FUNCTION_PROJECT=StatusPage.Function
FUNCTION_PATH=${FUNCTION_PROJECT}/out/${SERVICE_CONFIGURATION}

dotnet restore --configfile NuGet.Config
dotnet build --configuration ${SERVICE_CONFIGURATION} --no-restore /p:Version=${SERVICE_VERSION} /p:RepositoryUrl=${SERVICE_REPOSITORY_URL}
dotnet publish --no-build --output ${FUNCTION_PATH} ${FUNCTION_PROJECT}
dotnet test --configuration ${SERVICE_CONFIGURATION} --no-build --verbosity normal