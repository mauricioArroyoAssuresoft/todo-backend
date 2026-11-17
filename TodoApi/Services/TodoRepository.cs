using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services
{
  public class TodoRepository : ITodoRepository
  {
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
      _context = context;
    }

    public IEnumerable<TodoItem> GetAll()
    {
      List<TodoItem> todos = new()
      {
        new TodoItem("learn DI and IOC") {Id = 1},
      };

      return todos;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
      return await _context.Todos.AsNoTracking().ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
      return await _context.Todos.FindAsync(id);
    }

    public async Task<TodoItem> CreateAsync(TodoItem todo)
    {
      _context.Todos.Add(todo);
      await _context.SaveChangesAsync();
      return todo;
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var todo = await _context.Todos.FindAsync(id);
      if (todo is null)
      {
        return false;
      }

      _context.Todos.Remove(todo);
      await _context.SaveChangesAsync();
      return true;
    }
  }
}
