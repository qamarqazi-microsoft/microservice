using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory product model and store
var products = new List<Product>
{
    new Product(1, "Laptop", 999.99m),
    new Product(2, "Phone", 499.50m),
    new Product(3, "Headphones", 79.99m)
};

app.MapGet("/health", () => Results.Ok(new { status = "Product Service OK" }));

app.MapGet("/api/products", () => Results.Ok(products));

app.MapGet("/api/products/{id:int}", (int id) =>
{
    var p = products.FirstOrDefault(x => x.Id == id);
    return p is null ? Results.NotFound() : Results.Ok(p);
});

app.MapPost("/api/products", (Product product) =>
{
    products.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.Run();

// Record type declared after top-level statements
public record Product(int Id, string Name, decimal Price);
