FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src/YikesBot
COPY /src/YikesBot/* ./
RUN dotnet restore YikesBot.csproj
RUN dotnet build YikesBot.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish YikesBot.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YikesBot.dll"]
