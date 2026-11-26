using TodoApi.Domain.Entities;

namespace TodoApi.Application.Interfaces
{
  public interface ITodoRepository
  {
    Task<TodoItem> AddAsync(TodoItem todo);
  }
}
