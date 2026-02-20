# The 9 Books API

An API to retrieve hadith from nine famous Islamic books.

## Overview

| Book               | Book ID    | Hadith count |
| ------------------ | ---------- | ------------ |
| Sahih Bukhari      | bukhari    | 7008         |
| Sahih Muslim       | muslim     | 5362         |
| Sunan Nasai        | nasai      | 5662         |
| Sunan Abi Dawud    | abidawud   | 4590         |
| Sunan Tirmidhi     | tirmidhi   | 3891         |
| Sunan Ibn Majah    | ibnmaja    | 4332         |
| Muwatta Imam Malik | muwataa    | 1594         |
| Sunan Darimi       | darimi     | 3367         |
| Musnad Ahmad       | musnad     | 26363        |

## Getting Started

**For detailed setup instructions, please see [DOCKER_SETUP.md](DOCKER_SETUP.md)**

The project uses Docker for easy deployment. The Dockerfile automatically extracts the SQLite database from `SunnahDb.rar` during build (if `SunnahDb.db` is not already present).

### Obtaining the database

The database file is **not included** in the repository (it exceeds GitHub's file size limit). Before building with Docker (or running locally), you must have either:

- **SunnahDb.rar** – place it in `src/Api/`; the Dockerfile will extract it to `SunnahDb.db` during build, or
- **SunnahDb.db** – place it in `src/Api/` if you already have the SQLite file.

The data is derived from the [Open-Hadith-Data](https://github.com/mhashim6/Open-Hadith-Data) repository (CSV sources). You may need to build the database from those sources or obtain a pre-built file from a release or other source.

### Quick Start (Docker)

1. Clone the repository.
2. Obtain `SunnahDb.rar` (or `SunnahDb.db`) and place it in `src/Api/`.
3. Navigate to `src/Api` and build/run with Docker (see [DOCKER_SETUP.md](DOCKER_SETUP.md) for details).

### Local development (without Docker)

1. Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/3.1).
2. Place `SunnahDb.rar` or `SunnahDb.db` in `src/Api/`. If using the RAR, extract it to `SunnahDb.db` in the same folder.
3. From the repository root: `cd src/Api` then `dotnet run`.
4. The API runs at `http://localhost:5000` and `https://localhost:5001`; Swagger at `/swagger`, health at `/health`.

## Technical Details

- Built with .NET Core 3.1
- Uses SQLite database (`SunnahDb.db`)
- Database is not in the repo; use `SunnahDb.rar` (extracted during Docker build) or provide `SunnahDb.db`
- Original Hadith CSV sources: [Open-Hadith-Data](https://github.com/mhashim6/Open-Hadith-Data)

> **Note:** .NET Core 3.1 has reached end-of-life (Dec 2022) and no longer receives security updates. Consider upgrading to a supported .NET version for production use.

## Project Structure

```
The9Books/
├── The9Books.sln
├── README.md
├── DOCKER_SETUP.md
├── .gitignore
├── .dockerignore
└── src/Api/
    ├── Dockerfile
    ├── .dockerignore
    ├── The9Books.csproj
    ├── Program.cs
    ├── Startup.cs
    ├── SQLiteDBContext.cs
    ├── Random.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── SunnahDb.rar              # Database archive (not in repo)
    ├── Controllers/
    │   └── HadithController.cs
    ├── Models/
    │   ├── Hadith.cs             # DB entity
    │   ├── HadithDto.cs          # API response model
    │   ├── Book.cs
    │   ├── PagedResult.cs
    │   └── ApiSettings.cs
    ├── Middleware/
    │   └── ErrorHandlingMiddleware.cs
    └── Properties/
        └── launchSettings.json
```

## API Endpoints

### `GET /books`

Retrieves list of all books with their metadata.

### `GET /{bookId}/{hadithNumber}`

Retrieves a specific hadith from a specific book.

**Parameters:**

- `bookId`: Book identifier (see Book ID column above)
- `hadithNumber`: Hadith number (1-based)

**Example:** `GET /bukhari/1`

**Response:**

```json
{
  "number": 1,
  "hadith": "...",
  "tafseel": "...",
  "book": "bukhari"
}
```

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
