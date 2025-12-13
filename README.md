# The 9 Books API

An API to retrieve hadith from nine famous Islamic books.

## Overview

| Book               | Hadith count |
| ------------------ | ------------ |
| Sahih Bukhari      | 7008         |
| Sahih Muslim       | 5362         |
| Sunan Nasai        | 5662         |
| Sunan Abi Dawud    | 4590         |
| Sunan Tirmidhi     | 3891         |
| Sunan Ibn Majah    | 4332         |
| Muwatta Imam Malik | 1594         |
| Sunan Darimi       | 3367         |
| Musnad Ahmad       | 26363        |

## Getting Started

**For detailed setup instructions, please see [DOCKER_SETUP.md](DOCKER_SETUP.md)**

The project uses Docker for easy deployment. The Dockerfile automatically handles database extraction from the compressed `SunnahDb.rar` file.

### Quick Start

1. Clone the repository
2. Navigate to `src/Api` directory
3. Build and run with Docker (see [DOCKER_SETUP.md](DOCKER_SETUP.md) for details)

## Technical Details

- Built with .NET Core 3.1
- Uses SQLite database
- Database file is compressed as `SunnahDb.rar` (exceeds GitHub file size limit)
- The original Hadith CSV files can be found in [Open-Hadith-Data](https://github.com/mhashim6/Open-Hadith-Data) repository

## API Endpoints

### `GET /books`

Retrieves list of all books with their metadata.

### `GET /{bookId}/{hadithNumber}`

Retrieves a specific hadith from a specific book.

**Parameters:**

- `bookId`: Book identifier (e.g., "bukhari", "muslim")
- `hadithNumber`: Hadith number (1-based)

**Example:** `GET /bukhari/1`

### `GET /{bookId}/{startHadithNumber}/{rangeSize}`

Retrieves a paginated range of hadiths from a specific book.

**Parameters:**

- `bookId`: Book identifier
- `startHadithNumber`: Starting hadith number (1-based)
- `rangeSize`: Number of hadiths to retrieve (max 50, configurable)

**Response includes pagination metadata:**

- `data`: Array of hadiths
- `totalCount`: Total number of hadiths in the book
- `start`: Starting position
- `size`: Number of items returned
- `hasMore`: Whether more results are available

**Example:** `GET /bukhari/1/10`

### `GET /random`

Retrieves a random hadith from Sahih al-Bukhari (default).

### `GET /random/{bookId}`

Retrieves a random hadith from a specific book.

**Example:** `GET /random/muslim`

## API Documentation

Once the API is running, you can access:

- **Swagger UI**: `http://localhost:5000/swagger`
- **Health Check**: `http://localhost:5000/health`

## Additional Resources

- [Docker Setup Guide](DOCKER_SETUP.md) - Detailed instructions for running with Docker
- [Open-Hadith-Data](https://github.com/mhashim6/Open-Hadith-Data) - Original Hadith CSV files repository
