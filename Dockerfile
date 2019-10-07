FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /base
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS run
WORKDIR /app
COPY --from=publish /app .
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "Muse.dll"]