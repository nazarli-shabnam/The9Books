#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS builder
WORKDIR /source


COPY *.csproj .
RUN dotnet restore

COPY . ./

# Extract database from .rar if .db file doesn't exist
# This handles the case where only SunnahDb.rar is in the repository
RUN if [ ! -f SunnahDb.db ] && [ -f SunnahDb.rar ]; then \
        apt-get update && \
        apt-get install -y unrar && \
        unrar x SunnahDb.rar . && \
        apt-get remove -y unrar && \
        apt-get autoremove -y && \
        apt-get clean && \
        rm -rf /var/lib/apt/lists/*; \
    fi

RUN dotnet publish "./The9Books.csproj" --output "./dist" --configuration Release --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=builder /source/dist .

# Copy database file from builder stage
# The builder stage extracts from .rar if .db doesn't exist, so .db should be available
# If this fails, ensure SunnahDb.rar exists in src/Api/ - it will be extracted during build
COPY --from=builder /source/SunnahDb.db .

EXPOSE 80
ENTRYPOINT ["dotnet", "The9Books.dll"]