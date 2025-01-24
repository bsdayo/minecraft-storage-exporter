FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /repo

COPY ./src/MinecraftStorageExporter/MinecraftStorageExporter.csproj ./src/MinecraftStorageExporter/
RUN dotnet restore ./src/MinecraftStorageExporter/MinecraftStorageExporter.csproj

COPY . .
RUN dotnet publish ./src/MinecraftStorageExporter/MinecraftStorageExporter.csproj -o publish


FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

COPY --from=build /repo/publish .

ENTRYPOINT ["dotnet", "MinecraftStorageExporter.dll"]