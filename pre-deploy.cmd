dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\test\TauCode.Db.Tests\TauCode.Db.Tests.csproj
dotnet test -c Release .\test\TauCode.Db.Tests\TauCode.Db.Tests.csproj

nuget pack nuget\TauCode.Db.nuspec