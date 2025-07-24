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