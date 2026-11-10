
using TodoApi.Models;

namespace TodoApi.Services
{
  public class TodoRepository : ITodoRepository
  {
    public IEnumerable<TodoItem> GetAll()
    {
      List<TodoItem> todos = new()
      {
        new TodoItem("learn DI and IOC") {Id = 1},
      };

      return todos;
    }
  }
}
