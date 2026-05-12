FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ubersetzt.sln ./
COPY translator.Api/translator.Api.csproj translator.Api/
RUN dotnet restore translator.Api/translator.Api.csproj

COPY translator.Api/ translator.Api/
RUN dotnet publish translator.Api/translator.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

ENTRYPOINT ["dotnet", "translator.Api.dll"]
