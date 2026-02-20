# Docker Setup Guide

This guide will help you set up and run The 9 Books API using Docker.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running
- Git (to clone the repository)
- **Database file**: The repository does not include the database (it exceeds GitHub's file size limit). You must obtain **SunnahDb.rar** or **SunnahDb.db** and place it in `src/Api/` before building. The Dockerfile will extract `SunnahDb.rar` to `SunnahDb.db` during build if the `.db` file is not already present. See [README.md](README.md#obtaining-the-database) for more details.

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/MohamedAbdelghani/The9Books.git
cd The9Books
```

### 2. Obtain the Database and Navigate to API Directory

Place `SunnahDb.rar` (or `SunnahDb.db`) in `src/Api/`, then:

```bash
cd src/Api
```

**Important:** The Docker build must be run from the `src/Api` directory so that the Dockerfile can access the project and the database file.

### 3. Build the Docker Image

The Dockerfile will automatically extract the database from `SunnahDb.rar` if `SunnahDb.db` doesn't exist:

```bash
docker build -t 9books/api .
```

**Note**: The build process will:

- Use `SunnahDb.db` if present, or extract `SunnahDb.rar` to `SunnahDb.db` during build
- Build the .NET Core application
- Create a production-ready Docker image

### 4. Run the Container

```bash
docker run -d -p 5000:80 --name 9hadithbooks 9books/api
```

This command:

- Runs the container in detached mode (`-d`)
- Maps port 5000 on your host to port 80 in the container (`-p 5000:80`)
- Names the container `9hadithbooks` (`--name 9hadithbooks`)

### 5. Verify the API is Running

Wait a few seconds for the server to start, then test the API:

```bash
# Check health endpoint
curl http://localhost:5000/health

# Get list of books
curl http://localhost:5000/books

# Access Swagger UI
# Open http://localhost:5000/swagger in your browser
```

## Accessing the API

Once the container is running, you can access:

- **API Base URL**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`
- **Health Check**: `http://localhost:5000/health`

## API Endpoints

### Get All Books

```bash
GET http://localhost:5000/books
```

### Get Specific Hadith

```bash
GET http://localhost:5000/{bookId}/{hadithNumber}
```

Example:

```bash
GET http://localhost:5000/bukhari/1
```

### Get Range of Hadiths

```bash
GET http://localhost:5000/{bookId}/{start}/{size}
```

Example:

```bash
GET http://localhost:5000/bukhari/1/10
```

Returns paginated results with metadata:

```json
{
  "data": [...],
  "totalCount": 7008,
  "start": 1,
  "size": 10,
  "hasMore": true
}
```

### Get Random Hadith

```bash
GET http://localhost:5000/random
GET http://localhost:5000/random/{bookId}
```

Example:

```bash
GET http://localhost:5000/random/bukhari
```

## Container Management

### Stop the Container

```bash
docker stop 9hadithbooks
```

### Start the Container

```bash
docker start 9hadithbooks
```

### View Container Logs

```bash
docker logs 9hadithbooks
```

### View Real-time Logs

```bash
docker logs -f 9hadithbooks
```

### Remove the Container

```bash
docker stop 9hadithbooks
docker rm 9hadithbooks
```

### Remove the Image

```bash
docker rmi 9books/api
```

## Troubleshooting

### Port Already in Use

If port 5000 is already in use, use a different port:

```bash
docker run -d -p 8080:80 --name 9hadithbooks 9books/api
```

Then access the API at `http://localhost:8080`

### Database File Missing or Build Fails on COPY

The database is not in the repository. You must add it before building:

1. Obtain **SunnahDb.rar** or **SunnahDb.db** (see [README.md](README.md#obtaining-the-database)).
2. Place the file in `src/Api/` (same directory as the Dockerfile).
3. Run `docker build` from `src/Api/` (e.g. `cd src/Api` then `docker build -t 9books/api .`).
4. If extraction from RAR fails (e.g. `unrar` not found or archive format issue), manually extract `SunnahDb.rar` to `SunnahDb.db` in `src/Api/` and rebuild; the Dockerfile will use the existing `.db` file.

### Container Won't Start

Check the logs:

```bash
docker logs 9hadithbooks
```

Common issues:

- Database file not found
- Port conflict
- Insufficient memory

### Rebuild After Code Changes

If you make code changes:

```bash
# Stop and remove existing container
docker stop 9hadithbooks
docker rm 9hadithbooks

# Rebuild the image
docker build -t 9books/api .

# Run the new container
docker run -d -p 5000:80 --name 9hadithbooks 9books/api
```

## Environment Variables

The container runs in **Production** mode by default (`ASPNETCORE_ENVIRONMENT=Production`). You can override settings via environment variables:

```bash
docker run -d -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="Data Source=/data/SunnahDb.db" \
  -e ApiSettings__MaxPageSize=100 \
  --name 9hadithbooks 9books/api
```

Common variables:

| Variable | Default | Description |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Production | Set to `Development` for detailed error pages |
| `ConnectionStrings__DefaultConnection` | `Data Source=SunnahDb.db` | SQLite connection string |
| `ApiSettings__MaxPageSize` | `50` | Maximum page size for range queries |

## Development Workflow

For development, you might want to mount the source code as a volume for live reloading. However, for production use, the current setup is recommended.

## Production Considerations

- Use environment variables for configuration (see section above)
- Set up proper logging
- Configure CORS for your specific domains
- Use HTTPS in production (place a reverse proxy like nginx in front of the container)
- Set up health check monitoring on `/health`
- Consider using Docker Compose for multi-container setups

> **Note:** This project targets .NET Core 3.1, which reached end-of-life in December 2022 and no longer receives security patches. Consider upgrading to a supported .NET version before deploying to production.

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [.NET Core Docker Images](https://hub.docker.com/_/microsoft-dotnet-core)
- [API Documentation](README.md)

## Note on Building from the Correct Directory

Always run `docker build` from **inside `src/Api`** (e.g. `cd The9Books/src/Api` then `docker build -t 9books/api .`). The build context must include the project files and the database file (SunnahDb.rar or SunnahDb.db).
