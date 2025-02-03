FROM mcr.microsoft.com/dotnet/sdk:9.0.102-noble

# Installs the dotnet 8 runtime expected by dotnet test
COPY --from=mcr.microsoft.com/dotnet/sdk:8.0 /usr/share/dotnet/shared /usr/share/dotnet/shared

# Installs dependencies for AOT publish
RUN apt-get update && \
    apt-get install -y clang zlib1g-dev 
