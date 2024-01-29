using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.OpenApi.Models;
using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddDbContext<PizzaDB>(options => options.UseInMemoryDatabase("items"));
builder.Services.AddSqlite<PizzaDB>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "PizzaStore API", Description = "Making the Pizzas you love", Version = "V1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/pizzas", async (PizzaDB db) => await db.Pizzas.ToListAsync());
app.MapPost("/pizza", async (PizzaDB db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});
app.MapGet("/pizza/{id}", async (PizzaDB db, int id) => await db.Pizzas.FindAsync(id));
app.MapPut("/pizza/{id}", async (PizzaDB db, Pizza updatepizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    pizza.Name = updatepizza.Name;
    pizza.Description = updatepizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/pizza/{id}", async (PizzaDB db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null)
    {
        return Results.NotFound();
    }
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// app.MapGet("/pizzas/{id}", (int id) => PizzaDB.GetPizza(id));
// app.MapGet("/pizzas", () => PizzaDB.GetPizzas());
// app.MapPost("/pizzas", (Pizza pizza) => PizzaDB.CreatePizza(pizza));
// app.MapPut("/pizzas", (Pizza pizza) => PizzaDB.UpdatePizza(pizza));
// app.MapDelete("/pizzas/{id}", (int id) => PizzaDB.RemovePizza(id));

app.Run();
