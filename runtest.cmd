dotnet test Fanex.NCB.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet "./reportgenerator/4.8.10/tools/net5.0/ReportGenerator.dll" "-reports:./**/**/*.opencover.xml" "-targetdir:./CoverageReports" -reporttypes:Html
