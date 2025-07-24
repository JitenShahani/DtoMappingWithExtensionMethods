<!--
# 📚 Table of Contents

- [📘 Manual DTO Mapping via Extension Methods](#-manual-dto-mapping-via-extension-methods)
	- [🎯 Key Objectives](#-key-objectives)
	- [🗂️ Project Structure](#-project-structure)
	- [🧱 Startup Configuration](#-startup-configuration)
		- [🧱 `appsettings.json`](#-appsettings.json)
		- [🧱 `Program.cs`](#-program.cs)
    - [🛢️ Database Configuration](#-database-configuration)
        - [🗃️ `Data/ProductDbContext.cs`](#-dataproductdbcontext.cs)
    - [📦 Entities (`Entities/Product.cs`)](#-entities-entitiesproduct.cs)
        - [🧩 Key Points](#-key-points)
    - [📦 DTOs (`Dtos/ProductDtos.cs`)](#-dtos-dtosproductdtos.cs)
		- [🟢 Create Product Request DTO](#-create-product-request-dto)
		- [🟡 Update Product Request DTO](#-update-product-request-dto)
		- [🔵 Product Response DTO](#-product-response-dto)
    - [🏗️ Repository](#-repository)
		- [🧩 IProductRepository Interface](#-iproductrepository-interface)
		- [⚙️ Repository Implementation](#-repository-implementation)
        	- [🔍 Choosing your EF Core query strategy:](#-choosing-your-ef-core-query-strategy)
    - [🔧 Mapping Logic (`Mapping/ProductDtoExtensions.cs`)](#-mapping-logic-mappingproductdtoextensions.cs)
		- [🛠️ Entity to DTO: `Product → ProductResponse`](#-entity-to-dto-product-productresponse)
		- [🛠️ DTO to Entity: `CreateProductRequest → Product`](#-dto-to-entity-createproductrequest-product)
		- [🛠️ DTO to Entity: `UpdateProductRequest → Product`](#-dto-to-entity-updateproductrequest-product)
		- [🧩 Mapping Nested or Complex Types](#-mapping-nested-or-complex-types)
		- [❓ Why not use `AutoMapper` or similar libraries?](#-why-not-use-automapper-or-similar-libraries)
		- [🗂️ Organizing Mapping Logic as models grow](#-organizing-mapping-logic-as-models-grow)
    - [💻 Endpoints (`Endpoints/ProductEndpoints.cs`)](#-endpoints-endpointsproductendpoints.cs)
		- [🖥️ Endpoint Summary](#-endpoint-summary)
		- [💻 `GET /products`](#-get-products)
		- [💻 `GET /products/id`](#-get-productsid)
		- [💻 `POST /products`](#-post-products)
		- [💻 `PUT /products`](#-put-products)
		- [💻 `DELETE /products/id`](#-delete-productsid)
    - [🌐 Sample HTTP Requests](#-sample-http-requests)
        - [🌐 Get All Products (`GET /products`)](#-get-all-products-get-products)
			- [Sample Request](#sample-request)
			- [Sample Response](#sample-response)
        - [🌐 Get Product (`GET /products/id:Guid`)](#-get-product-get-productsidguid)
			- [Sample Request](#sample-request)
			- [Sample Response](#sample-response)
        - [🌐 Create a Product (`POST /products`)](#-create-a-product-post-products)
			- [Sample Request](#sample-request)
			- [Sample Response](#sample-response)
        - [🌐 Update a Product (`PUT /products`)](#-update-a-product-put-products)
			- [Sample Request](#sample-request)
			- [Sample Response](#sample-response)
        - [🌐 Delete a Product (`DELETE /products/id:Guid`)](#-delete-a-product-delete-productsidguid)
            - [Sample Request](#sample-request)
	- [🔄 End-to-End Request Pipeline](#-end-to-end-request-pipeline)
	- [🚨 Common Pitfalls](#-common-pitfalls)
	- [✅ Best Practices](#-best-practices)
	- [📚 References](#-references)
-->

# 📘 Manual DTO Mapping via Extension Methods

[![Microsoft.AspNetCore.OpenApi](https://img.shields.io/nuget/dt/Microsoft.AspNetCore.OpenApi.svg?label=Microsoft.AspNetCore.OpenApi&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/)
[![Microsoft.EntityFrameworkCore](https://img.shields.io/nuget/dt/Microsoft.EntityFrameworkCore.svg?label=Microsoft.EntityFrameworkCore&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)
[![Microsoft.EntityFrameworkCore.Sqlite](https://img.shields.io/nuget/dt/Microsoft.EntityFrameworkCore.Sqlite.svg?label=Microsoft.EntityFrameworkCore.Sqlite&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/)
[![Microsoft.EntityFrameworkCore.Tools](https://img.shields.io/nuget/dt/Microsoft.EntityFrameworkCore.Tools.svg?label=Microsoft.EntityFrameworkCore.Tools&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/)

This project showcases manual DTO mapping in ASP.NET Core Web API using extension methods. It emphasizes clarity, separation of concerns, and explicit control over data transformation. By avoiding external mapping libraries, the code remains transparent, purposeful, and easy to evolve. The structure highlights how to shape request and response models, configure serialization, and organize endpoint logic around minimal APIs and clean repository abstraction.

## 🎯 Key Objectives

- ✔️ Demonstrate clean and explicit DTO mapping without external libraries.
- ✔️ Maintain separation between domain models, transport models, and persistence logic.
- ✔️ Use extension methods for consistent and readable data transformation.
- ✔️ Structure endpoints around Minimal APIs using clear route definitions and typed results.
- ✔️ Configure JSON serialization for safety and predictable contract delivery.
- ✔️ Persist data using EF Core with SQLite in a local development-friendly setup.

## 🗂️ Project Structure

This solution is organized for clarity and maintainability. Each folder encapsulates a distinct responsibility, from endpoint design and manual data transformation to repository abstraction and database interaction, making the codebase easy to navigate and extend.

```plaintext
├── ManualDtoMappingDemo
│   ├── Data
│   │   └── ProductDB.db			# SQLite Database file for local development
│   │   └── ProductDbContext.cs			# EF Core DbContext configured for SQLite
│   ├── Dtos
│   │   └── ProductDtos.cs			# DTO definitions for create, update, and response
│   │   	└── CreateProductRequest	# Input model for product creation
│   │   	└── UpdateProductRequest	# Input model for product update
│   │   	└── ProductResponse		# Output model returned to clients
│   ├── Endpoints
│   │   └── ProductEndpoints.cs			# Minimal API endpoints grouped under `/products`
│   ├── Entities
│   │   └── Product.cs				# Domain model representing the Product entity
│   ├── Mapping
│   │   └── ProductDtoExtensions.cs		# Extension methods for mapping between entity and DTO
│   ├── Repositories
│   │   ├── IProductRepository.cs		# Repository interface for CRUD operations
│   │   └── ProductRepository.cs		# EF Core implementation of the repository pattern
│   ├── appsettings.json			# JSON config including SQLite connection string
│   └── Program.cs				# Main entry point. Application startup, service registration, and endpoint mapping
```

## 🧱 Startup Configuration

All service registrations and application setup logic are defined in `Program.cs`. This includes configuration for JSON serialization, EF Core with SQLite, scoped repository services, and OpenAPI tooling.

- 📦 `JsonOptions`

	Configured via `builder.Services.Configure<JsonOptions>` to customize serialization behavior:
	- Ignores null values when writing JSON (`DefaultIgnoreCondition`).
	- Uses Pascal casing for property names and dictionary keys (`PropertyNamingPolicy` and `DictionaryKeyPolicy`).
	- Enables case-insensitive property matching during deserialization.
	- Handles circular references using `ReferenceHandler.IgnoreCycles`.
- 🗃️ EF Core with SQLite

	Registered via `builder.Services.AddDbContext<ProductDbContext>`, with connection string loaded from `appsettings.json`:
- 🏗️ Repository Pattern

	Scoped repository services are registered to allow dependency injection in endpoints, ensuring clean separation of concerns.
- 📘 OpenAPI Integration

	Minimal OpenAPI support enabled through `builder.Services.AddOpenApi()` and `app.MapOpenApi()` in development.
- 🔐 HTTPS Redirection

	Configured with `app.UseHttpsRedirection()` to enforce secure requests.
- 🧩 Endpoint Mapping

	Routes are grouped and registered using `ProductEndpoints.MapProductEndpoints(app)` to keep definitions modular and encapsulated.

### 🧱 `appsettings.json`

```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		}
	},
	"AllowedHosts": "*",
	"ConnectionStrings": {
		"Database": "Data Source=Data\\ProductDB.db"
	}
}
```

### 🧱 `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder (args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi ();

builder.Services.Configure<JsonOptions> (options =>
{
	// Configure JSON serializer to ignore null values during serialization
	options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

	// Configure JSON serializer to use Pascal case for property names during serialization
	options.SerializerOptions.PropertyNamingPolicy = null;

	// Configure JSON serializer to use Pascal case for key's name during serialization
	options.SerializerOptions.DictionaryKeyPolicy = null;

	// Ensure JSON property names are not case-sensitive during deserialization
	options.SerializerOptions.PropertyNameCaseInsensitive = true;

	// Prevent serialization issues caused by cyclic relationships in EF Core entities
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

	// Ensure the JSON output is consistently formatted for readability.
	// Not to be used in Production as the response message size could be large
	// options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddDbContext<ProductDbContext> (options =>
{
	options.UseSqlite (builder.Configuration.GetConnectionString ("Database"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository> ();

var app = builder.Build ();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ())
{
	app.MapOpenApi ();
}

app.UseHttpsRedirection ();

new ProductEndpoints ().MapProductEndpoints (app);

app.Run ();
```

## 🛢️ Database Configuration

This project uses SQLite as a lightweight, embedded database ideal for local development and testing. EF Core handles schema generation and persistence automatically based on the `Product` entity.

- 📂 Database File Location

	The database file is created at `Data\ProductDB.db` upon application startup.

- 📄 Connection String

	Defined in `appsettings.json` under the `ConnectionStrings:Database` key.

- 🗃️ EF Core Setup

	SQLite is registered as the provider through `AddDbContext<ProductDbContext>()`. The corresponding `DbContext` declares a single `DbSet<Product>` property for interacting with the `Products` table.

- 🧪 Development Friendly

	Because the database is file-based and versioned locally, it's easy to reset or inspect during iterative development.

### 🗃️ `Data/ProductDbContext.cs`

```csharp
public class ProductDbContext (DbContextOptions<ProductDbContext> options) : DbContext (options)
{
	public DbSet<Product> Products { get; set; }
}
```

## 📦 Entities (`Entities/Product.cs`)

The `Product` class represents the domain entity for this demo. It defines the core business data that gets persisted in the SQLite database via EF Core. This entity is intentionally simple, focusing on essential product fields.

```csharp
public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

### 🧩 Key Points

- `Id` is a globally unique identifier for the product.
- `Name`, `Description`, `Quantity`, and `Price` represent core product attributes.
- `CreatedAt` is set at the time of instantiation, providing audit context.
- EF Core maps this class to the Products table using `DbSet<Product>` in `ProductDbContext`.

This entity is mapped to external contracts through extension methods, ensuring that internal fields like CreatedAt are excluded unless explicitly needed.

## 📦 DTOs (`Dtos/ProductDtos.cs`)

The `ProductDtos.cs` file defines three record types that shape request and response payloads. Each DTO serves a distinct purpose in endpoint communication, avoiding direct exposure of domain entities.

### 🟢 Create Product Request DTO

```csharp
public sealed record CreateProductRequest (string Name, string Description, int Quantity, decimal Price);
```

> 📝 Used in the `POST /products` endpoint. Maps to the `Product` entity via extension methods and defines the shape of client input during creation. It omits `Id` and `CreatedAt`, as those values are generated by the server.

### 🟡 Update Product Request DTO

```csharp
public sealed record UpdateProductRequest (Guid Id, string Name, string Description, int Quantity, decimal Price);
```

> 📝 Used in the `PUT /products` endpoint. Includes the `Id` of the record being updated. Allows overwriting all editable product fields.

### 🔵 Product Response DTO

Returned to clients in response payloads. Exposes key product info along with server-assigned metadata like `Id` while excluding `CreatedAt`.

```csharp
public sealed record ProductResponse (Guid Id, string Name, string Description, int Quantity, decimal Price);
```

> 📝 Used as the response model exclusively in `GET /products` and `GET /products/{id}` endpoints. Intentionally excludes internal fields like `CreatedAt` for a focused contract.

## 🏗️ Repository

The repository layer encapsulates data access logic, abstracting EF Core interactions behind a clean interface. This promotes separation of concerns and simplifies testing and endpoint composition.

### 🧩 IProductRepository Interface

Defines the contract for CRUD operations related to the `Product` entity.

```csharp
public interface IProductRepository
{
	Task<IEnumerable<Product>> GetAllAsync (CancellationToken cancellationToken);
	Task<Product?> GetByIdAsync (Guid id, CancellationToken cancellationToken);
	Task AddAsync (Product product, CancellationToken cancellationToken);
	Task UpdateAsync (Product product, CancellationToken cancellationToken);
	Task DeleteAsync (Product product, CancellationToken cancellationToken);
}
```

> 📝 Promotes consistency and testability by abstracting persistence concerns. Injected into endpoints for direct use without exposing `DbContext`.

### ⚙️ Repository Implementation

Implements the `IProductRepository` interface using EF Core's `ProductDbContext`.

```csharp
public class ProductRepository : IProductRepository
{
	private readonly ProductDbContext _dbContext;

	public ProductRepository (ProductDbContext dbContext)
		=> _dbContext = dbContext;

	public async Task<IEnumerable<Product>> GetAllAsync (CancellationToken cancellationToken)
	{
		return await _dbContext.Products
			.AsNoTracking()
			.ToListAsync (cancellationToken);
	}

	public async Task<Product?> GetByIdAsync (Guid id, CancellationToken cancellationToken)
	{
		return await _dbContext.Products
			.AsNoTracking ()
			.FirstOrDefaultAsync (p => p.Id == id, cancellationToken);
	}

	public async Task AddAsync (Product product, CancellationToken cancellationToken)
	{
		_dbContext.Products.Add (product);
		await _dbContext.SaveChangesAsync (cancellationToken);
	}

	public async Task UpdateAsync (Product product, CancellationToken cancellationToken)
	{
		await _dbContext.Products
			.Where (p => p.Id == product.Id)
			.ExecuteUpdateAsync (
				setters => setters
					.SetProperty (p => p.Name, product.Name)
					.SetProperty (p => p.Description, product.Description)
					.SetProperty (p => p.Quantity, product.Quantity)
					.SetProperty (p => p.Price, product.Price)
					.SetProperty (p => p.CreatedAt, product.CreatedAt),
				cancellationToken);
	}

	public async Task DeleteAsync (Product product, CancellationToken cancellationToken)
	{
		await _dbContext.Products
			.Where (p => p.Id == product.Id)
			.ExecuteDeleteAsync (cancellationToken);
	}
}
```

> 📝 **GET methods** Use EF Core's `AsNoTracking()` for efficient read-only access. Primary key lookup is performed using `FirstOrDefaultAsync()` with a predicate filter.

> 📝 `ExecuteUpdateAsync()` and `ExecuteDeleteAsync()` directly apply changes without using EF's change tracker so no `SaveChangesAsync()` required. These direct SQL execution methods require EF Core 7.0 or later.

#### 🔍 Choosing your EF Core query strategy:

EF Core offers two ways to retrieve entities by primary key, each with distinct behavior and tradeoffs:

- `FindAsync(id)`

	- 🔹 **Behavior**: Uses internal tracking and key metadata to locate entities.
	- ✅ **Pros**: Fast for primary keys. May return cached entities if already tracked.
	- ❌ **Cons**: Doesn't support `.AsNoTracking()` or eager loading. Less flexible for queries.

- `FirstOrDefaultAsync(...)`

	- 🔹 **Behavior**: Executes full LINQ query with filter predicates.
	- ✅ **Pros**: Works with `.AsNoTracking()`. Extendable with includes, joins, and filters.
	- ❌ **Cons**: Slightly slower for pure key lookups. Always hits the database.
	- ✅ This demo intentionally uses `FirstOrDefaultAsync()` for predictable behavior, explicit filtering, and clean separation from EF Core’s change tracking especially in read-only scenarios.

> Other query methods exist, but these two are the most common for primary key lookups in typical CRUD APIs.

This layer is registered as a scoped dependency in `Program.cs`, ensuring a fresh context per request and promoting thread safety during data operations.

## 🔧 Mapping Logic (`Mapping/ProductDtoExtensions.cs`)

This section defines extension methods used to convert between domain entities and DTOs. Manual mapping ensures precise control over which fields are exposed or consumed, reinforcing separation between internal models and transport contracts.

### 🛠️ Entity to DTO: `Product → ProductResponse`

```csharp
	public static ProductResponse ToDto (this Product product)
		=> new (product.Id, product.Name, product.Description, product.Quantity, product.Price);
```

> 📝 Used in the `GET /products` and `GET /products/{id}` endpoints to return a clean representation of product data to clients. Internal fields like `CreatedAt` are intentionally excluded.

### 🛠️ DTO to Entity: `CreateProductRequest → Product`

```csharp
public static Product ToEntity (this CreateProductRequest request)
	=> new ()
	{
		Name = request.Name,
		Description = request.Description,
		Quantity = request.Quantity,
		Price = request.Price,
		CreatedAt = DateTime.Now
	};
```

> 📝 Used in the `POST /products` endpoint. Creates a new domain entity with a generated `Id` and default `CreatedAt`. Ensures the request model is correctly translated for persistence.

### 🛠️ DTO to Entity: `UpdateProductRequest → Product`

```csharp
public static Product ToEntity (this UpdateProductRequest request, Product existingProduct)
	=> new()
	{
		Id = request.Id,
		Name = request.Name,
		Description = request.Description,
		Quantity = request.Quantity,
		Price = request.Price,

		// Since, PUT updates the entire record, we need to preserve the CreatedAt column value.
		// Otherwise, it will be overwritten with a blank value since it is not part of the UpdateProductRequest.
		// This is a common pattern to ensure audit fields remain intact during updates.
		CreatedAt = existingProduct.CreatedAt
	};
```

> 📝 Used in the `PUT /products` endpoint. Reconstructs the product entity based on the incoming update payload. Caller is responsible for ensuring `Id` validity and preserving audit fields like `CreatedAt`.

These extension methods are lightweight, readable, and easy to locate. They embody the principle of explicit transformation while keeping the mapping logic decoupled from both DTOs and entities.

### 🧩 Mapping Nested or Complex Types

While this demo uses flat DTOs and entities, manual mapping scales to more complex scenarios, such as:

- Entities with nested objects or collections (e.g., `Order` with `OrderItems`).
- DTOs that flatten or reshape data for specific API contracts.

For nested types, you can compose mapping methods:

```csharp
// Example: mapping an Order entity with nested OrderItems
public static OrderResponse ToDto(this Order order) =>
    new(
        order.Id,
        order.CustomerName,
        order.Items.Select(item => item.ToDto()).ToList()
    );
```

This approach keeps mapping logic explicit and testable, even as models grow in complexity.

### ❓ Why not use `AutoMapper` or similar libraries?

- **Transparency**: Manual mapping makes every transformation explicit and easy to debug.
- **Performance**: Libraries like `AutoMapper` use reflection, which can impact performance, especially with large or complex object graphs.
- **Control**: You avoid accidental field exposure and have full control over contract evolution.
- **Future-proofing**: Relying on third-party libraries for core logic can introduce risks if the library changes license, becomes unsupported, or introduces breaking changes.

Manual mapping is easy to unit test. Each mapping method is a simple function and can be tested independently, ensuring correctness as your models evolve.

### 🗂️ Organizing Mapping Logic as models grow

As your application grows and the number of DTOs/entities increases, consider:

- Organizing extension methods by feature or domain (e.g., separate files per aggregate or module).
- Using namespaces to group related mappers.
- Keeping mapping logic close to the models they transform for discoverability.

## 💻 Endpoints (`Endpoints/ProductEndpoints.cs`)

This section defines the Minimal API endpoints grouped under `/products`. Each handler leverages manual DTO mapping and the repository pattern to process product-related operations. Mapping logic is centralized in `ProductDtoExtensions.cs` for clarity and consistency.

### 🖥️ Endpoint Summary

| Endpoint Route	| HTTP Method | Accepts					| Returns													|
|-------------------|-------------|-------------------------|-----------------------------------------------------------|
| `/products`		| GET		  | —						| `IEnumerable<ProductResponse>?`							|
| `/products/{id}`	| GET		  | Guid (Route)			| ProductResponse?											|
| `/products`		| POST		  | CreateProductRequest 	| Guid + Response header named `Location`					|
| `/products`		| PUT		  | UpdateProductRequest	| "Update Successful" in plain text							|
| `/products/{id}`	| DELETE	  | Guid (Route)			| 404 Not Found or 204 No Content if deleted successfully	|

### 💻 `GET /products`

Fetches all products from the database and returns them as a list of `ProductResponse`.

```csharp
var productGroup = app.MapGroup ("/products");

productGroup.MapGet ("/", async (
	IProductRepository productRepository,
	CancellationToken cancellationToken) =>
{
	var products = (await productRepository.GetAllAsync (cancellationToken))
		.Select (p => p.ToDto ());

	return TypedResults.Ok (products);
});
```

> 📝 Returns a list of products using `ProductResponse` DTO for output. Mapping is performed via `.ToDto()` extension method making sure internal entity fields like `CreatedAt` remain excluded.

### 💻 `GET /products/id`

Fetches a single product by its `Id`. Returns `NotFound` if no match is found.

```csharp
var productGroup = app.MapGroup ("/products");

productGroup.MapGet ("/{id:Guid}", async (
	[FromRoute] Guid id,
	IProductRepository productRepository,
	CancellationToken cancellationToken) =>
{
	var product = await productRepository.GetByIdAsync (id, cancellationToken);

	return product is null
		? Results.NotFound ()
		: TypedResults.Ok (product.ToDto ());
}).WithName ("GetProductById");
```

> 📝 Returns a single product using `ProductResponse` DTO for output. Mapping is performed via `.ToDto()` extension method making sure internal entity fields like `CreatedAt` remain excluded.

### 💻 `POST /products`

Creates a new product. The client sends a `CreateProductRequest` DTO.

```csharp
var productGroup = app.MapGroup ("/products");

productGroup.MapPost ("/", async (
		[FromBody] CreateProductRequest request,
		IProductRepository productRepository,
		HttpContext httpContext,
		CancellationToken cancellationToken) =>
	{
		var product = request.ToEntity ();

		await productRepository.AddAsync (product, cancellationToken);

		var uri = httpContext
			.RequestServices
			.GetRequiredService<LinkGenerator> ()
			.GetUriByName (httpContext, "GetProductById", new { product.Id });

		return TypedResults.Created (uri, product.Id);
	});
```

> 📝 The request is mapped to a `Product` entity using the `.ToEntity()` extension method. Only the generated `Id` is returned in the response body (not a full DTO), and the `Location` header is set for easy retrieval of the created resource.

### 💻 `PUT /products`

Fully updates an existing product using the `UpdateProductRequest` DTO. Returns status messages based on operation outcome.

```csharp
var productGroup = app.MapGroup ("/products");

productGroup.MapPut ("/", async (
	[FromBody] UpdateProductRequest request,
	IProductRepository productRepository,
	CancellationToken cancellationToken) =>
{
	if (request.Id == Guid.Empty)
		return Results.BadRequest ("Product Id is required");

	var existingProduct = await productRepository.GetByIdAsync (request.Id, cancellationToken);

	if (existingProduct is null)
		return Results.NotFound ();

	await productRepository.UpdateAsync(request.ToEntity(existingProduct), cancellationToken);

	return TypedResults.Ok ("Update Successful");
});
```

> 📝 The entity is reconstructed manually using `.ToEntity()` extension method and `CreatedAt` is preserved. A plain text status is returned. For missing or unknown IDs, appropriate HTTP response is generated (`400 Bad Request`, `404 Not Found`).

### 💻 `DELETE /products/id`

Deletes a product by its `Id`. Returns `NoContent` if successful.

```csharp
var productGroup = app.MapGroup ("/products");

productGroup.MapDelete ("/{id:Guid}", async (
	[FromRoute] Guid id,
	IProductRepository productRepository,
	CancellationToken cancellationToken) =>
{
	var product = await productRepository.GetByIdAsync (id, cancellationToken);

	if (product is null)
		return Results.NotFound ();

	await productRepository.DeleteAsync (product, cancellationToken);

	return TypedResults.NoContent ();
});
```

> 📝 Operates directly on entity from repository. No DTOs used. Ensures safe deletion with null check.

## 🌐 Sample HTTP Requests

📄 Request samples sourced from [`ManualDtoMappingDemo.http`](/ManualDtoMappingDemo/ManualDtoMappingDemo.http)

This section demonstrates practical request/response samples for each endpoint in the `/products` group. It reinforces DTO usage and expected behavior without diving into source code.

```http
@HostAddress = http://localhost:5122
```

### 🌐 Get All Products (`GET /products`)

#### Sample Request

```http
GET {{HostAddress}}/products
Content-Type: none

###
```

#### Sample Response

```json
[
    {
        "Id": "0196960d-0c2c-7f11-a2c9-96023bfd93c3",
        "Name": "Gaming Laptop",
        "Description": "A high-performance laptop with advanced graphics for gaming.",
        "Quantity": 10,
        "Price": 999.99
    },
    {
        "Id": "0196960d-4839-7971-8eb7-a37633230f2b",
        "Name": "Gaming Mice",
        "Description": "A high-performance low-latency light-wight mice for gaming.",
        "Quantity": 22,
        "Price": 68.99
    },
    {
        "Id": "0196960d-9e4e-7585-9c74-86f60257d0e0",
        "Name": "Dummy Product",
        "Description": "This is a dummy product.",
        "Quantity": 1,
        "Price": 12.99
    },
    {
        "Id": "ee909367-476a-431e-a80c-d720770df8e7",
        "Name": "SK Hynix Internal SSD",
        "Description": "SK Hynix Gold P31 1TB PCIe NVMe Gen3 M.2 2280 Internal SSD read up to 3500MB/s and write up to 3200MB/s.",
        "Quantity": 1,
        "Price": 119.99
    },
    {
        "Id": "edc75d6b-b2c9-4d3a-83ca-3d9c81468521",
        "Name": "Temp Product",
        "Description": "This is a temp product",
        "Quantity": 1,
        "Price": 14.99
    }
]
```

> 📝 Returns a list of products using `ProductResponse` DTO. Internal fields like `CreatedAt` are excluded.

### 🌐 Get Product (`GET /products/id:Guid`)

#### Sample Request

```http
GET {{HostAddress}}/products/0196960d-9e4e-7585-9c74-86f60257d0e0
Content-Type: none

###
```

#### Sample Response

```json
{
	"Id": "0196960d-9e4e-7585-9c74-86f60257d0e0",
	"Name": "Dummy Product",
	"Description": "This is a dummy product.",
	"Quantity": 1,
	"Price": 12.99
}
```

> 📝 Returns a single product using `ProductResponse` DTO. Internal fields like `CreatedAt` are excluded.

### 🌐 Create a Product (`POST /products`)

#### Sample Request

```http
POST {{HostAddress}}/products
Content-Type: application/json

{
  "Name": "SK Hynix Internal SSD",
  "Description": "SK Hynix Gold P31 1TB PCIe NVMe Gen3 M.2 2280 Internal SSD read up to 3500MB/s and write up to 3200MB/s.",
  "Quantity": 1,
  "Price": 119.99
}

###
```

#### Sample Response

```plaintext
"ee909367-476a-431e-a80c-d720770df8e7"
```

> 📝 Consumes `CreateProductRequest` DTO and returns newly generated `Id` and `Location` header. Response body contains the Guid.

### 🌐 Update a Product (`PUT /products`)

#### Sample Request

```http
PUT {{HostAddress}}/products/
Content-Type: application/json

{
  "id": "0196960d-4839-7971-8eb7-a37633230f2b",
  "name": "Gaming Mice",
  "description": "A high-performance low-latency light-wight mice for gaming.",
  "quantity": 22,
  "price": 68.99
}

###
```

#### Sample Response

```plaintext
"Update Successful"
```

> 📝 Consumes `UpdateProductRequest` DTO and returns `Update Successful` response as plain-text in case the update was successful. Minimal APIs wrap string responses in quotes (`"..."`) with `Content-Type: text/plain`. Returns `400 Bad Request` along with a string `Product Id is required` if the `Id` is missing from request payload and returns `404 Not Found` if the product with the specified `Id` does not exist.

### 🌐 Delete a Product (`DELETE /products/id:Guid`)

#### Sample Request

```http
DELETE {{HostAddress}}/products/5c492ba0-3d44-402a-966e-56b578cf0648
Content-Type: none

###
```

> 📝 Deletes the specified product by `Id`. Returns `204 No Content` on success, or `404 Not Found` if the product with the specified `Id` does not exist.

## 🔄 End-to-End Request Pipeline

Let’s walk through how a client interacts with the API when creating or updating a product, tracing the request through mapping, persistence, and response delivery.

```
[1] 📨 Client sends HTTP request
	└── Example:
		- GET /products
		- GET products/0196960d-9e4e-7585-9c74-86f60257d0e0
		- POST /products with JSON payload
		{
			"Name": "SK Hynix Internal SSD",
			"Description": "SK Hynix Gold P31 1TB PCIe NVMe Gen3 M.2 2280 Internal SSD read up to 3500MB/s and write up to 3200MB/s.",
			"Quantity": 1,
			"Price": 119.99
		}
		- PUT /products with JSON payload
		{
			"id": "0196960d-4839-7971-8eb7-a37633230f2b",
			"name": "Gaming Mice",
			"description": "A high-performance low-latency light-wight mice for gaming.",
			"quantity": 22,
			"price": 68.99
		}

	✅ The shape of the request is defined by CreateProductRequest or UpdateProductRequest DTO.

[2] 🛂 Minimal API endpoint receives the request
	└── Endpoint reads the DTO via [FromBody] binding
	└── Calls .ToEntity() extension method to transform DTO → Product entity

	✅ Explicit and manual mapping ensures control over data flow and avoids accidental exposure of internal fields.

[3] 🧩 Extension method maps DTO to entity
	└── For CreateProductRequest, CreatedAt is initialized inside the mapper
	└── For UpdateProductRequest, CreatedAt is preserved manually by reading the original entity from the database

	✅ Mapping logic lives in ProductDtoExtensions.cs to keep separation of concerns.

[4] 🗃️ Repository processes the entity
	└── AddAsync() calls EF Core’s .Add() and uses SaveChangesAsync()
	└── UpdateAsync() uses ExecuteUpdateAsync() for a direct SQL update with no change tracking
	└── DeleteAsync() uses ExecuteDeleteAsync() for efficient hard deletion

	✅ Repository abstracts persistence details via IProductRepository interface.

[5] 🗄️ SQLite database is updated
	└── EF Core translates the entity into SQL statements
	└── Entity is written to Data\ProductDB.db

	✅ EF Core translates entity changes into SQL statements and persists them to the SQLite database.

[6] 📤 Server returns appropriate response
	◀── Might be:
		- ✅ 201 Created + Location header (on successful POST)
		- ✅ "Update Successful" string (on successful PUT)
		- ❌ 400 Bad Request if payload Id is missing
		- ❌ 404 Not Found if requested resource is unavailable
		- ✅ 204 No Content if delete succeeds

	✅ All responses follow Minimal API conventions.
```


> 🧭 This pipeline highlights how the application transforms transport-level DTOs into domain entities and then persists them safely ensuring clear contracts, safe updates, and intentional error handling.

## 🚨 Common Pitfalls

While the demo emphasizes clarity and intentional design, here are some common mistakes to watch for when building production-grade APIs:

1. **Over-mapping or redundant logic in endpoints**

	Avoid repeating transformation steps or placing mapping code inline with endpoint logic. Centralize it via extension methods to improve readability and maintenance.

2. **Reusing entities directly in transport contracts**

	Domain entities often contain audit fields, tracking flags, or relationships that shouldn't be exposed externally. Use DTOs to shield internal structure.

3. **Forgetting to handle null or default values in serializers**

	Configure JSON options explicitly to prevent unexpected behavior like serializing nulls or misaligned property casing with client expectations.

4. **Reusing entities as DTOs**

	This leads to field leakage, especially for properties like `CreatedAt` or EF navigation properties. Keep models intentionally scoped for their role.

5. **Skipping validation on client input**

	Without proper validation, incorrect or partial data can reach your database, leading to long-term inconsistency and business rule violations.

6. **Overwriting entire entity during updates**

	Blindly replacing all fields can unintentionally erase important data. Always preserve audit fields (`CreatedAt`) and consider partial updates where appropriate.

7. **Forgetting to name routes for resource creation**

	Named routes make it easier to generate Location URIs after `POST` requests. Without them, you'll need to hardcode paths or skip confirmation headers.

8. **Assuming serialization defaults match frontend expectations**

	Contract alignment matters. Explicitly configure casing and null behavior so the frontend and backend remain predictable and interoperable.

> 🧭 These pitfalls surface most often when layering complexity on top of simple APIs. Staying intentional with each design choice keeps your application safe, predictable, and easy to evolve.

## ✅ Best Practices

This section outlines principles that promote clarity, safety, and maintainability when building Minimal API applications with manual DTO mapping.

1. **Prefer explicit over automatic mapping**

	Manual mapping via extension methods ensures full control over how data moves between layers. While packages like `AutoMapper` can reduce boilerplate, they introduce complexity that may not be justified in smaller projects.

	❌ Risks with automatic mapping:
	- Relies on reflection and runtime analysis, which can affect performance, especially in high-throughput or startup-sensitive applications.
	- Requires configuration to handle edge cases, increasing cognitive overhead.
	- Convention-based mappings can silently include or exclude fields, making behavior less predictable and harder to debug.

	✅ Why manual mapping excels:
	- Behavior is transparent, expressive, and intentionally scoped.
	- Mapping logic lives beside your domain types for easy discoverability.
	- Eliminates surprises. Every field is transformed deliberately, reducing risk of unintended data exposure or contract mismatch.

	Manual mapping aligns perfectly with small-to-mid sized APIs where clarity, control, and maintainability are more valuable than abstraction.

2. **Separate transport contracts from domain models**

	DTOs (Data Transfer Objects) act as the shape of data exchanged between clients and servers. They are designed to capture just what's necessary for input (requests) or output (responses), shielding internal logic, persistence details, and sensitive metadata.

	🛡️ Why separation matters:
	- Domain entities often contain fields like `CreatedAt`, audit flags, relationships, or EF Core navigation properties, which may be irrelevant, sensitive, or unstable across deployments.
	- Returning entities directly can leak internal implementation or cause serialization issues. For example, cyclic references or unexpected field exposure.
	- Using entities as input models creates coupling to database structure, which increases the risk of invalid data manipulation, accidental overwrites, or unintended migrations.

	✅ Benefits of using purpose-built DTOs:
	- You control the surface area of the API contract explicitly.
	- Changes to domain models don’t ripple into transport contracts.
	- Requests stay lean, responses remain intentional, and versioning becomes easier over time.
	- Mapping DTOs manually ensures domain logic stays protected and properly enriched before persistence.

	This pattern reinforces architectural boundaries, treating your API as a curated interface, not a transparent mirror of your database schema.

3. **Preserve audit fields during updates**

	Audit fields (e.g., `CreatedAt`, `ModifiedAt`, `CreatedBy`) are typically assigned by the server to track lifecycle events of a record. When performing updates, especially with DTOs that represent only editable properties, it's essential to preserve these values to maintain historical accuracy and data integrity.

	❌ Common pitfalls:
	- Overwriting the entire entity using `PUT` or `UpdateAsync()` without rehydrating server-assigned fields.
	- Relying solely on DTO input which lacks fields like `CreatedAt`, resulting in those being reset or lost.

	✅ Recommended approach:
	- Always fetch the original entity before applying updates.
	- Copy over immutable fields (`CreatedAt`) from the existing record manually.
	- Keep audit logic out of DTOs. It’s a responsibility of the mapping layer or domain rules.

	This safeguard ensures that each update maintains consistency with the original creation context which is especially critical in logging, compliance, or historical reporting scenarios.

4. **Centralize transformation logic in mapping methods**

	Mapping between DTOs and domain entities is a repetitive operation in most APIs. Centralizing this logic using dedicated extension methods or helper functions ensures consistency and reduces code duplication across endpoint handlers.

	❌ Risks of inline or scattered mapping:
	- Makes endpoint code noisy and harder to follow.
	- Increases chances of subtle inconsistencies (e.g., forgetting to set a field or initializing timestamps incorrectly).
	- Mapping logic becomes harder to test or reuse independently.

	✅ Benefits of centralized mapping methods:
	- Promotes separation of concerns, endpoints handle routing and orchestration, while mappers handle transformation.
	- Encourages reuse, especially when DTOs are consumed across multiple endpoints.
	- Supports cleaner unit tests. You can test mapping methods in isolation without spinning up the entire API.
	- Makes debugging and enhancement easier. When domain changes occur, only the mappers need updating.

	By keeping transformation logic in well-named methods (like `.ToEntity()` or `.ToDto()`), your codebase remains expressive, maintainable, and resilient to change. All with minimal friction.

5. **Favor `TypedResults` for consistent responses**

    Typed results in ASP.NET Core’s Minimal APIs, like `TypedResults.Ok(...)`, `TypedResults.Created(...)`, and `TypedResults.NoContent()` provide explicit, strongly-typed response contracts. They enhance discoverability through IntelliSense, reduce ambiguity, and reinforce semantic correctness across your endpoints.

    ❌ Risks of returning anonymous or untyped results:
    - Potential for inconsistent response formats across endpoints.
    - Harder to trace and refactor when business logic grows.
    - Ambiguous return values (e.g., raw strings or tuples) can cause confusion downstream or when generating OpenAPI metadata.

    ✅ Why typed results matter:
    - Encourage precise pairing of status codes with content. For example, `Created(...)` always implies `201` and can include a Location header.
    - Improve IDE tooling support, surfacing available response types as suggestions during development.
    - Reduce accidental mismatches between payloads and status codes (e.g., returning JSON with `NoContent`).
    - Make unit testing and mocking easier by exposing response types explicitly.

    Typed results offer both clarity and intent. When combined with purpose-built DTOs and meaningful status codes, they help create APIs that are not just functional but predictable, educational, and easy to integrate.

6. **Design endpoints with clear route grouping and names (recommended design choice)**

	Using `MapGroup()` to group related routes under a common prefix (`/products`, `/orders`, etc.) improves organization and helps developers navigate endpoint definitions quickly. Naming routes with `.WithName()` is optional but beneficial when you need to generate URLs dynamically such as in a `Created(...)` response for a `POST` request.

	🛠️ When is it optional?
	- For small internal APIs or prototypes, explicit names and grouping may feel verbose.
	- If you’re not generating URIs or using reverse routing, `.WithName()` might be skipped safely.

	✅ Benefits of this approach:
	- Keeps related routes visually and structurally connected.
	- Makes your routing logic modular and easier to extend.
	- Allows precise URI generation using `LinkGenerator.GetPathByName(...)`, `LinkGenerator.GetUriByName(...)`, or tag helpers.
	- Enables reverse routing without hardcoded paths especially useful when returning `Location` headers.

	This pattern strikes a balance between **structure** and **flexibility**. It's not required, but it’s one of those small practices that pays dividends in readability and long-term usability.

7. **Configure JSON serialization consciously**

	ASP.NET Core provides a highly customizable JSON serialization pipeline via `JsonOptions`, powered by `System.Text.Json`, offering fine-grained control over how JSON is shaped and interpreted. The current configuration which sets casing policies, null handling, reference management, and case insensitivity forms an excellent baseline for predictable contracts.

	You can configure these options for Controllers using `AddControllers()` in `Program.cs`.

	```csharp
	builder.Services.AddControllers().AddJsonOptions(options =>
	{
		// Configure JSON serializer to ignore null values during serialization
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

		// Configure JSON serializer to use Pascal case for property names during serialization
		options.JsonSerializerOptions.PropertyNamingPolicy = null;

		// Configure JSON serializer to use Pascal case for key's name during serialization
		options.JsonSerializerOptions.DictionaryKeyPolicy = null;

		// Ensure JSON property names are not case-sensitive during deserialization
		options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

		// Prevent serialization issues caused by cyclic relationships in EF Core entities
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

		// Ensure the JSON output is consistently formatted for readability.
		// Not to be used in Production as the response message size could be large
		// options.JsonSerializerOptions.WriteIndented = true;
	});
	```

	✅ Key configurations in this demo:
	- **Pascal casing preservation** via `PropertyNamingPolicy = null` and `DictionaryKeyPolicy = null`.
	- **Null value suppression** using `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull`.
	- **Cycle avoidance** through `ReferenceHandler.IgnoreCycles`.
	- **Case-insensitive deserialization** with `PropertyNameCaseInsensitive = true`.

	📌 Additional configuration options to consider as APIs grow:
	- `WriteIndented = true` (for pretty-printing during development or debugging).
	- Custom converters (e.g., for enums, `DateTime`, or polymorphic types).
	- Control over number formatting and precision (e.g., decimal rounding).
	- Default value handling and conditional serialization.
	- Support for camelCase when integrating with JavaScript-heavy clients.

	❌ Risks of relying on defaults:
	- Clients may experience unexpected casing or missing fields.
	- Circular references can cause runtime errors if not handled early.
	- Lack of configuration can limit contract clarity and increase onboarding friction.

	Making your intentions explicit in `Program.cs` strengthens communication between backend and frontend teams and keeps your contract behavior discoverable.

8. **Validate client input before persistence**

	While this demo intentionally skips full-fledged input validation to keep the focus on DTO mapping and endpoint structure, validation remains an essential step in production scenarios.

	✅ Simple checks worth adding later:
	- Ensure identifiers (e.g., `Guid.Id`) are not empty.
	- Confirm required fields are present and logically valid.
	- Block negative or out-of-range values that violate business rules.

	🛡️ Why it matters:
	- Prevents incorrect or incomplete data from reaching the database.
	- Enables early error feedback to clients, improving usability.
	- Helps enforce business rules and maintain long-term data integrity.

9. **Use NoTracking on read queries**

	When retrieving data that won’t be updated during the current request, using `AsNoTracking()` tells Entity Framework Core not to track changes to the returned entities. This reduces memory usage, avoids unnecessary change detection, and improves overall query performance.

	❌ Risks of not using it:
	- Unintended database updates if modified entities are flushed with `SaveChangesAsync()`.
	- Larger memory footprint, especially with complex graphs or high-volume queries.
	- Reduced query performance in read-heavy endpoints.

	✅ Why this is beneficial:
	- EF Core skips building the change tracker graph, lowering CPU and memory overhead.
	- Eliminates accidental updates. Data is treated as read-only, so even if modified in memory, it won’t be persisted unless explicitly reattached.
	- Ideal for endpoints like `GET /products` or `GET /products/{id}`, where the response is strictly informational.

	🧠 When to avoid `AsNoTracking()`:
	- If you plan to modify and persist the entity within the same request.
	- When lazy loading or navigation properties require tracking behavior.

	Using `AsNoTracking()` isn’t just an optimization, it reinforces the intent of “read-only” access, making your API behavior cleaner and safer by design.

10. **Document expected request/response shapes**

	An API isn’t just a functional interface, it’s a contract between backend and client. By documenting sample requests and responses clearly, you help consumers understand the shape, expectations, and flow of data before they even touch the code.

	✅ Why this matters:
	- Reduces guesswork for frontend developers and third-party integrators.
	- Clarifies which fields are required, what output looks like, and how to structure calls.
	- Accelerates onboarding and testing by giving real-world examples that can be run or adapted instantly.

	📄 How this demo supports it:
	- Includes realistic request and response examples for each endpoint using `.http` file and inline samples.
	- Demonstrates actual `Guid` IDs, DTO structures, and returned payloads.
	- Uses commentary to explain status codes, formats, and Minimal API conventions (e.g., why string responses are wrapped in quotes).

	Well-crafted samples turn documentation from reference material into an invitation to explore and this project showcases that with precision.

> 🧭 Following these practices ensures your Minimal APIs remain easy to reason about, safe for consumers, and ready for growth as complexity increases.

## 📚 References

These resources provide official guidance and additional context behind the concepts demonstrated in this project:

- [Minimal APIs in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
    Overview of Minimal API routing, endpoint design, and response conventions.

- [Using Extension Methods in C#](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
    Language feature that powers manual mapping between DTOs and entities.

- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core)
    Covers repository setup, `AsNoTracking`, and database operations via EF Core.

- [System.Text.Json Configuration](https://learn.microsoft.com/en-us/dotnet/api/system.text.json)
    Reference for JSON serializer settings including casing, null handling, and reference behavior.

- [OpenAPI Integration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-9.0)
    Setup and configuration tips for enabling API documentation using Swashbuckle.

> 📎 These links support the architectural clarity emphasized in this demo and offer next steps for deeper exploration.

---

**_Stay Curious. Build Thoughtfully_**
