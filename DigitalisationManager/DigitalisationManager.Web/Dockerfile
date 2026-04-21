# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DigitalisationManager.Web/DigitalisationManager.Web.csproj", "DigitalisationManager.Web/"]
COPY ["DigitalisationManager.Data.Models/DigitalisationManager.Data.Models.csproj", "DigitalisationManager.Data.Models/"]
COPY ["DigitalisationManager.GCommon/DigitalisationManager.GCommon.csproj", "DigitalisationManager.GCommon/"]
COPY ["DigitalisationManager.Data/DigitalisationManager.Data.csproj", "DigitalisationManager.Data/"]
COPY ["DigitalisationManager.Services.Core/DigitalisationManager.Services.Core.csproj", "DigitalisationManager.Services.Core/"]
COPY ["DigitalisationManager.Web.ViewModels/DigitalisationManager.Web.ViewModels.csproj", "DigitalisationManager.Web.ViewModels/"]
RUN dotnet restore "./DigitalisationManager.Web/DigitalisationManager.Web.csproj"
COPY . .
WORKDIR "/src/DigitalisationManager.Web"
RUN dotnet build "./DigitalisationManager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DigitalisationManager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalisationManager.Web.dll"]