# ---- Etapa 1: BUILD ----
# Usa la imagen oficial del SDK de .NET 8 para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todos los .csproj y restauramos dependencias (cachea mejor)
COPY ["SGV.API/SGV.API.csproj", "SGV.API/"]
COPY ["SGV.Business/SGV.Business.csproj", "SGV.Business/"]
COPY ["SGV.Data/SGV.Data.csproj", "SGV.Data/"]
COPY ["SGV.DTOs/SGV.DTOs.csproj", "SGV.DTOs/"]
COPY ["SGV.Entities/SGV.Entities.csproj", "SGV.Entities/"]

RUN dotnet restore "SGV.API/SGV.API.csproj"

# Copiamos el resto del código y publicamos
COPY . .
RUN dotnet publish "SGV.API/SGV.API.csproj" -c Release -o /app/publish

# ---- Etapa 2: RUNTIME ----
# Imagen más liviana, solo para ejecutar (no incluye el SDK completo)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "SGV.API.dll"]
