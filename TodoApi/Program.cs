using Microsoft.EntityFrameworkCore;
using TodoApi;
using TodoApi.Models;
using TodoApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

TodoEndpoints.Map(app);

app.Run();
