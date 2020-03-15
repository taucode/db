dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\tests\TauCode.Db.Tests\TauCode.Db.Tests.csproj
dotnet test -c Release .\tests\TauCode.Db.Tests\TauCode.Db.Tests.csproj

nuget pack nuget\TauCode.Db.nuspec