dotnet pack DpdtInject.Generator\DpdtInject.Generator.csproj -p:TargetFrameworks=netstandard2.0

xcopy /Y DpdtInject.Generator\bin\Debug\*.nupkg .