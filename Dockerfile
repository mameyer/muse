FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Muse.sln .
COPY src/Muse/*.csproj ./src/Muse/
COPY src/SpotifyApi/*.csproj ./src/SpotifyApi/
COPY src/SpotifyApi/src/SpotifyApi.NetCore/*.csproj /source/src/SpotifyApi/src/SpotifyApi.NetCore/
COPY src/BackgroundServices/*.csproj ./src/BackgroundServices/
COPY src/TSGenerator/*.csproj ./src/TSGenerator/
COPY src/TSGeneratorTests/*.csproj ./src/TSGeneratorTests/
RUN dotnet restore

RUN mkdir local_packages
RUN dotnet pack src/TSGenerator/TSGenerator.csproj
RUN cp src/TSGenerator/bin/Release/*.nupkg local_packages 
RUN dotnet restore srs/TSGeneratorTests/TSGeneratorTests.csproj

# copy everything else and build app
COPY . .
WORKDIR /source/Muse
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Muse.dll"]