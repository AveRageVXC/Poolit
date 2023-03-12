FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /DBMigrations
COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS release
WORKDIR /DBMigrations
COPY --from=build out .
ENTRYPOINT ["dotnet", "DBMigrations.dll", "Release"]