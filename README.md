# RecipeShare API

A RESTful API for sharing and managing recipes, built with ASP.NET Core 9.0 and Entity Framework Core.

## Features

- **CRUD Operations:** Full Create, Read, Update, Delete functionality for recipes
- **Data Validation:** Server-side validation with FluentValidation
- **API Documentation:** Swagger/OpenAPI integration
- **Database:** SQL Server with Entity Framework Core migrations
- **Testing:** Integration tests with xUnit and SQLite in-memory database
- **Performance:** Benchmark-tested concurrent request handling

## API Endpoints

| Method | Endpoint              | Description |
|--------|----------------------|-------------|
| GET    | `/api/recipes`       | Get all recipes |
| GET    | `/api/recipes/{id}`  | Get recipe by ID |
| POST   | `/api/recipes`       | Create new recipe |
| PUT    | `/api/recipes/{id}`  | Update existing recipe |
| DELETE | `/api/recipes/{id}`  | Delete recipe by ID |

## Performance Benchmarks

Tested on Intel Celeron N4020 (2 cores, 1.1GHz) with .NET 9.0.5

### 500 GET /api/recipes Requests

| Method                            | Mean       | Speedup vs Sequential | Memory Used |
|----------------------------------|------------|----------------------|-------------|
| **500 Sequential Requests**      | **1,640 ms** | **Baseline**         | **2.14 MB** |
| **500 Requests in Batches of 50**| **807 ms**  | **2.03x faster**     | **2.21 MB** |
| **500 Requests with 20 Concurrent** | **824 ms** | **1.99x faster**   | **2.30 MB** |

### Smaller Load Test

| Method                   | Mean     | Speedup vs Sequential | Memory Used |
|--------------------------|----------|----------------------|-------------|
| **100 Concurrent Requests** | **193 ms** | **8.49x faster**  | **0.47 MB** |

### Performance Analysis

- **Sequential Performance:** ~3.3 ms per request - Excellent response time for basic hardware
- **Concurrency Benefits:** Batch processing achieves 2x speedup, demonstrating effective concurrent request handling
- **Scalability:** The API maintains consistent memory usage across all load patterns
- **Resource Management:** Semaphore-limited concurrency prevents system overload while maximizing throughput
- **Real-world Applicability:** 20 concurrent requests is realistic for small-to-medium recipe sharing applications

The API demonstrates robust performance characteristics suitable for production use with multiple concurrent users.

## Testing

- **Integration Tests:** 100% coverage of all CRUD endpoints with validation
- **Test Database:** SQLite in-memory for fast, isolated test execution
- **Test Results:** All 4 test cases pass with proper HTTP status codes and data validation

## Tech Stack

- **Backend:** ASP.NET Core 9.0 Web API
- **Database:** SQL Server 2022 with Entity Framework Core 9.0
- **Validation:** FluentValidation
- **Testing:** xUnit + SQLite in-memory
- **Performance:** BenchmarkDotNet
- **Documentation:** Swagger/OpenAPI

## Setup

1. **Clone and Restore:**
   ```bash
   git clone https://github.com/AndileDimba/RecipeShare.git
   cd RecipeShare
   dotnet restore