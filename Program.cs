using EF_StoreProcedure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultEF_StoreProcedureConnection");

builder.Services.AddDbContext<EF_StoreProcedureDbContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, CancellationToken) =>
    {
        document.Info.Version = "v1";
        document.Info.Title = "Entity Framework Transaction Implementation";
        document.Info.Description = "A transaction in any database system represents a logical unit of work, a set of operations that must succeed or fail together. When an application performs multiple related operations (e.g., creating an order, inserting order items, updating stock, creating payment records), these operations must be treated as one atomic process.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Store Procedure");
        options.WithTheme(ScalarTheme.DeepSpace);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
