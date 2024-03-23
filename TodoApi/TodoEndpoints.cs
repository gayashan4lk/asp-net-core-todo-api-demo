using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Persistence;

namespace TodoApi;

public static class TodoEndpoints
{
    public static void Map(WebApplication app)
    {
        var todosApi = app.MapGroup("/todos");
        
        todosApi.MapGet("/", GetAllTodos);
        todosApi.MapGet("/complete", GetCompletedTodos);
        todosApi.MapGet("/{id}", GetTodo);
        todosApi.MapPost("/", CreateTodo);
        todosApi.MapPut("/{id}", UpdateTodo);
        todosApi.MapDelete("/{id}", DeleteTodo);
    }
    
    static async Task<IResult> GetAllTodos(TodoDb db)
    {
        var todos = await db.Todos.ToListAsync();
        var result = TypedResults.Ok(todos);
        return result;
    }

    static async Task<IResult> GetCompletedTodos(TodoDb db)
    {
        var todos = await db.Todos.Where(t => t.IsComplete).ToListAsync();
        var result = TypedResults.Ok(todos);
        return result;
    }
    
    static async Task<IResult> GetTodo(int id, TodoDb db)
    {
        var item = await db.Todos.FindAsync(id);
        return item is Todo todo ? Results.Ok(todo) : Results.NotFound();
    }
    
    static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return Results.Created($"/todos/{todo.Id}", todo);
    }
    
    static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return Results.NotFound();

        todo.Name = inputTodo.Name;
        todo.IsComplete = inputTodo.IsComplete;

        await db.SaveChangesAsync();

        return Results.NoContent();
    }
    
    static async Task<IResult> DeleteTodo(int id, TodoDb db)
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }

}