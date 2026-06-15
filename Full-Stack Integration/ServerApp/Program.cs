// InventoryHub - Minimal API Back-End
// AI assisted with: CORS setup, nested JSON structure design, and IMemoryCache caching pattern.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

// AI suggestion: register IMemoryCache to avoid rebuilding the product list on every request
builder.Services.AddMemoryCache();

var app = builder.Build();

// AI helped resolve CORS errors by suggesting AllowAnyOrigin for development environments
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

// AI generated the nested Category structure and recommended the caching pattern below
app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    const string cacheKey = "productlist";

    // Return cached result if available, avoiding redundant object creation on the server
    if (cache.TryGetValue(cacheKey, out object? cachedProducts))
        return cachedProducts;

    var products = new[]
    {
        new
        {
            Id = 1,
            Name = "Laptop",
            Price = 1200.50,
            Stock = 25,
            Category = new { Id = 101, Name = "Electronics" }
        },
        new
        {
            Id = 2,
            Name = "Headphones",
            Price = 50.00,
            Stock = 100,
            Category = new { Id = 102, Name = "Accessories" }
        }
    };

    // Cache for 5 minutes to minimise server load under repeated requests
    cache.Set(cacheKey, products, TimeSpan.FromMinutes(5));

    return products;
});

app.Run();
