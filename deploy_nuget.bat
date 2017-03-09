call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat"

nuget push src\UNM.Parser\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package
del src\UNM.Parser\bin\Release\*.nupkg

nuget push src\UNM.GCS\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package
del src\UNM.GCS\bin\Release\*.nupkg

echo "Deployment Complete"