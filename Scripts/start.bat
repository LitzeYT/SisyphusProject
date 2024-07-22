setlocal ENABLEDELAYEDEXPANSION
set curdir=%cd%
set dir=%curdir:\Scripts=!""!%

cd %dir%

echo "Build Server"
call dotnet build .\SisyphusServer\SisyphusServer.sln

echo "Start containers" 
call docker-compose -f .\Docker\docker-compose.yml --env-file .\Docker\.env-dev up -d --build --force-recreate 
