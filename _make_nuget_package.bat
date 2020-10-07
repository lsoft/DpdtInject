dotnet pack DpdtInject.Generator\DpdtInject.Generator.csproj -p:TargetFrameworks=netstandard2.0 -c:Release

xcopy /Y DpdtInject.Generator\bin\Release\*.nupkg .