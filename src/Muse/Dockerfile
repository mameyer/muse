#FROM microsoft/dotnet:3.0-aspnetcore-runtime AS final
#FROM microsoft/dotnet:3.0-sdk AS final
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base

# WORKDIR /app
# COPY ./bin/Release/netcoreapp3.0/publish/. .

# EXPOSE 80

# ENTRYPOINT ["dotnet", "Muse.dll"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Muse/Muse.csproj", "src/Muse/"]
RUN dotnet restore "src/Muse/Muse.csproj"
COPY . .
WORKDIR "/src/src/Muse"
RUN dotnet build "Muse.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Muse.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Muse.dll"]
