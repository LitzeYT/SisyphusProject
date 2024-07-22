FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /Sisyphus
COPY ./bin/Debug/net8.0 .

EXPOSE 8080/tcp 443/tcp
ENTRYPOINT [ "dotnet", "SisyphusServer.dll" ]
