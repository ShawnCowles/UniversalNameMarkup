call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86_amd64

cd src\UNM.Parser

nuget pack UNM.Parser.csproj -Symbols -Build -Properties Configuration=Release;Platform=AnyCpu

REM nuget push *.nupkg -Source https://www.nuget.org/api/v2/package

del *.nupkg

pause