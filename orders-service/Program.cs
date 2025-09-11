using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// configure typed HttpClient for Products service
builder.Services.AddHttpClient("products", client =>
{
    // The base address will be overridden by docker-compose when in containers.
    client.BaseAddress = new Uri(builder.Configuration["ProductsService__BaseUrl"] ?? "http://localhost:5001");
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Orders OK" }));

var orders = new List<Order>();
var nextOrderId = 1;

app.MapPost("/api/orders", async (HttpContext http, IHttpClientFactory httpClientFactory) =>
{
    var orderRequest = await http.Request.ReadFromJsonAsync<OrderItem[]>();
    if (orderRequest == null || !orderRequest.Any())
    {
        return Results.BadRequest("No items");
    }

    var client = httpClientFactory.CreateClient("products");

    decimal total = 0m;
    var validatedItems = new List<OrderItem>();

    foreach (var item in orderRequest)
    {
        var resp = await client.GetAsync($"/api/products/{item.ProductId}");
        if (!resp.IsSuccessStatusCode)
        {
            return Results.BadRequest($"Product {item.ProductId} not found");
        }

        var product = await resp.Content.ReadFromJsonAsync<ProductDto>();
        if (product is null)
        {
            return Results.BadRequest("Invalid product response");
        }

        total += product.Price * item.Quantity;
        validatedItems.Add(item);
    }

    var order = new Order(nextOrderId++, validatedItems, total);
    orders.Add(order);
    return Results.Created($"/api/orders/{order.Id}", order);
});

app.MapGet("/api/orders", () => Results.Ok(orders));

app.MapGet("/api/orders/{id:int}", (int id) =>
{
    var o = orders.FirstOrDefault(x => x.Id == id);
    return o is null ? Results.NotFound() : Results.Ok(o);
});

app.Run();

// record types go at the bottom
public record OrderItem(int ProductId, int Quantity);
public record Order(int Id, IEnumerable<OrderItem> Items, decimal Total);
public record ProductDto(int Id, string Name, decimal Price);
