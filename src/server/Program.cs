using Microsoft.EntityFrameworkCore;
using Npgsql;
using server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddDbContext<TodoDb>(options => options.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=admin;"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//var connectionString = "Host=localhost;Username=postgres;Password=admin;Database=postgres";
//await using var dataSource = NpgsqlDataSource.Create(connectionString);

//await dataSource.OpenConnectionAsync();
//await using var command = dataSource.CreateCommand("SELECT rolname FROM pg_roles");
//await using var reader = await command.ExecuteReaderAsync();

//while (await reader.ReadAsync())
//{
//    Console.WriteLine(reader.GetString(0));
//}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TodoDb>();
    context.Database.EnsureCreated();
}


//app.UseHttpsRedirection();
app.UseCors(x => {
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
    x.AllowAnyMethod();
});


app.MapGet("/todos", async (TodoDb db)=> await db.Todods.ToListAsync());


app.MapPost("/todos", async (Todo todo, TodoDb db) =>
{
    await db.Todods.AddAsync(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPatch("/todos/{id}", async (int id, Todo inputTodo, TodoDb db) => 
{ 
    var todo = await db.Todods.FindAsync(id);
    if(todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsCompleted = inputTodo.IsCompleted;
    await db.SaveChangesAsync();
    return Results.NoContent();

});

app.MapDelete("/todos/{id}", async (int id, TodoDb db) => 
{
    var todo = await db.Todods.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();


public class Todo
{
    public int Id { get; set; } 
    public string? Name { get; set;  } 
    public bool IsCompleted { get; set;  } 
}