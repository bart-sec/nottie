using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Npgsql;
using server;

var builder = WebApplication.CreateBuilder(args);
var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
var keycloakClientId = builder.Configuration["Keycloak:ClientId"];

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nottie",
        Version = "v1",
    });

    //define auth 2.0
    c.AddSecurityDefinition(nameof(SecuritySchemeType.OAuth2), new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                { {"openid", "OpenID Connect Scope" },
                    {"profile", "User profile" }
                }
            }
        }
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference(nameof(SecuritySchemeType.OAuth2), doc), []
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"]!;
        options.Audience = builder.Configuration["Keycloak:Audience"];

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Keycloak:Issuer"]
        };

        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddDbContext<TodoDb>(options => options.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=admin;"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId(keycloakClientId);
        options.OAuthUsePkce();
    });
    //app.MapOpenApi();
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
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(x =>
{
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
    x.AllowAnyMethod();
});


app.MapGet("/todos", async (TodoDb db) => await db.Todods.ToListAsync())
    .RequireAuthorization();


app.MapPost("/todos", async (Todo todo, TodoDb db) =>
{
    await db.Todods.AddAsync(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todos/{todo.Id}", todo);
}).RequireAuthorization();

app.MapPatch("/todos/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todods.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsCompleted = inputTodo.IsCompleted;
    await db.SaveChangesAsync();
    return Results.NoContent();

}).RequireAuthorization();

app.MapDelete("/todos/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todods.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();


public class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsCompleted { get; set; }
}