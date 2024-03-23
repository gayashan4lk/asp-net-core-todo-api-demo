using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var todosApi = app.MapGroup("/todos");

app.MapGet("/", () => "Hello World!");

todosApi.MapGet("/", async (TodoDb db) => await db.Todos.ToListAsync());
todosApi.MapGet("/complete", async (TodoDb db) => await db.Todos.Where(t => t.IsComplete).ToListAsync());
todosApi.MapGet("/{id}", async (int id, TodoDb db) =>
{
    var item = await db.Todos.FindAsync(id);
    return item is Todo todo ? Results.Ok(todo) : Results.NotFound();
});

todosApi.MapPost("/", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

todosApi.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

todosApi.MapDelete("/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();