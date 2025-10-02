using Microsoft.EntityFrameworkCore;
using RecipeShare.Api.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? "Server=localhost,14333;Database=RecipeShareDb;User Id=sa;Password=Your_str0ng!Pass;TrustServerCertificate=True;";
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<RecipeDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(conn)));
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
    // db.Database.Migrate(); //this gave me errors when using SQLite InMemory for testing but I will leave it
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }