using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Persistence;

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options): base(options){}

    public DbSet<Todo> Todos => Set<Todo>();
}