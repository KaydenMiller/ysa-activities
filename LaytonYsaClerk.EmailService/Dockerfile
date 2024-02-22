FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LaytonYsaClerk.EmailService/LaytonYsaClerk.EmailService.csproj", "LaytonYsaClerk.EmailService/"]
RUN dotnet restore "LaytonYsaClerk.EmailService/LaytonYsaClerk.EmailService.csproj"
COPY . .
WORKDIR "/src/LaytonYsaClerk.EmailService"
RUN dotnet build "LaytonYsaClerk.EmailService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LaytonYsaClerk.EmailService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LaytonYsaClerk.EmailService.dll"]
