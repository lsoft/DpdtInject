REM Clear the 3.x+ cache (use either command)
dotnet nuget locals http-cache --clear
nuget locals http-cache -clear

REM Clear the 2.x cache (NuGet CLI 3.5 and earlier only)
nuget locals packages-cache -clear

REM Clear the global packages folder (use either command)
dotnet nuget locals global-packages --clear
nuget locals global-packages -clear

REM Clear the temporary cache (use either command)
dotnet nuget locals temp --clear
nuget locals temp -clear

REM Clear the plugins cache (use either command)
dotnet nuget locals plugins-cache --clear
nuget locals plugins-cache -clear

REM Clear all caches (use either command)
dotnet nuget locals all --clear
nuget locals all -clear
