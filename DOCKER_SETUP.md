# Docker Setup Guide

This guide will help you set up and run The 9 Books API using Docker.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running
- Git (to clone the repository)

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/MohamedAbdelghani/The9Books.git
cd The9Books
```

### 2. Navigate to API Directory

```bash
cd src/Api
```

### 3. Build the Docker Image

The Dockerfile will automatically extract the database from `SunnahDb.rar` if `SunnahDb.db` doesn't exist:

```bash
docker build -t 9books/api .
```

**Note**: The build process will:

- Extract `SunnahDb.rar` to `SunnahDb.db` automatically if needed
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

### Database File Missing

If you see errors about the database file:

1. Ensure `SunnahDb.rar` exists in `src/Api/` directory
2. The Dockerfile will automatically extract it during build
3. If extraction fails, manually extract `SunnahDb.rar` to `SunnahDb.db` before building

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

## Development Workflow

For development, you might want to mount the source code as a volume for live reloading. However, for production use, the current setup is recommended.

## Production Considerations

- Use environment variables for configuration
- Set up proper logging
- Configure CORS for your specific domains
- Use HTTPS in production
- Set up health check monitoring
- Consider using Docker Compose for multi-container setups

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [.NET Core Docker Images](https://hub.docker.com/_/microsoft-dotnet-core)
- [API Documentation](README.md)
