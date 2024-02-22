FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LaytonYSAClerk.Cli/LaytonYSAClerk.Cli.csproj", "LaytonYSAClerk.Cli/"]
RUN dotnet restore "LaytonYSAClerk.Cli/LaytonYSAClerk.Cli.csproj"
COPY . .
WORKDIR "/src/LaytonYSAClerk.Cli"
RUN dotnet build "LaytonYSAClerk.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LaytonYSAClerk.Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LaytonYSAClerk.Cli.dll"]
