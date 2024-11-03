echo off
set sonar_projectkey="techchallange-phase1"
set sonar_analysistoken="sqa_8a85e1fda02aee04e1b907d865991acf0b8f6bf5"
set sonar_url="http://[2804:7f2:8280:9065:25af:e575:b9cd:713c]:9000"

echo on

dotnet tool install --global dotnet-sonarscanner
dotnet sonarscanner begin /k:%sonar_projectkey% /d:sonar.token=%sonar_analysistoken% /d:sonar.host.url=%sonar_url%  /d:sonar.cs.opencover.reportsPaths=**/coverage.opencover.xml /d:sonar.coverage.exclusions=**/Migrations/*.cs,Program.cs /d:sonar.qualitygate.wait=true /d:sonar.scanner.scanAll=false
dotnet build
dotnet test --no-build --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
dotnet sonarscanner end /d:sonar.token=%sonar_analysistoken%