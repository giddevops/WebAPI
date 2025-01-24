REM dotnet publish --configuration "Release"
msbuild WebApi.csproj /p:DeployOnBuild=true /p:PublishProfile=Properties\PublishProfiles\GIDGideonWebApiFTP.pubxml
