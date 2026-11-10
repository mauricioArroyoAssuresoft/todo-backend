
using TodoApi.Models;

namespace TodoApi.Services
{
  public interface ITodoRepository
  {
    IEnumerable<TodoItem> GetAll();
  }
}
