FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /Poolit
COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS release
WORKDIR /Poolit
COPY --from=build out .
ENTRYPOINT ["dotnet", "Poolit.dll"]
