# start portainer
docker run -d -p 8000:8000 -p 9000:9000 -v /var/run/docker.sock:/var/run/docker.sock -v portainer_data:/data portainer/portainer

cd /home/mmeyer/projects/Muse/src/Muse

dotnet restore

dotnet build

# publishes to 'bin/Release/netcoreapp3.0/publish/'
dotnet publish --configuration Release

docker build -t muse .