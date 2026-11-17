
using TodoApi.Models;

namespace TodoApi.Services
{
  public interface ITodoRepository
  {
    IEnumerable<TodoItem> GetAll();
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task<TodoItem> CreateAsync(TodoItem todo);
    Task<bool> DeleteAsync(int id);
  }
}
