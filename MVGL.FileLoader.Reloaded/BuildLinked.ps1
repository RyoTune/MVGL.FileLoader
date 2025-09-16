# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/MVGL.FileLoader.Reloaded/*" -Force -Recurse
dotnet publish "./MVGL.FileLoader.Reloaded.csproj" -c Release -o "$env:RELOADEDIIMODS/MVGL.FileLoader.Reloaded" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location