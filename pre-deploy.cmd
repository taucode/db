dotnet restore

dotnet build TauCode.Db.sln -c Debug
dotnet build TauCode.Db.sln -c Release

dotnet test TauCode.Db.sln -c Debug
dotnet test TauCode.Db.sln -c Release

nuget pack nuget\TauCode.Db.nuspec