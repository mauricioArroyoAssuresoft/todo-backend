using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class TodosController : ControllerBase
  {
    private static readonly List<TodoItem> Todos = new()
    {
      new TodoItem("buy groceries") { Id = 1},
      new TodoItem("learn rest apis") {Id = 2}
    };
    private readonly ILogger<TodosController> _logger;

    public TodosController(ILogger<TodosController> logger)
    {
      _logger = logger;
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
      _logger.LogInformation("LOGGER: checking api health status");
      return Ok(new { status = "asp.net core web api is running" });
    }

    [HttpGet]
    public ActionResult<IEnumerable<TodoItem>> GetAll()
    {
      return Ok(Todos);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TodoItem> GetById(int id)
    {
      var todo = Todos.FirstOrDefault(t => t.Id == id);
      if (todo == null)
      {
        return NotFound(new { message = $"todo item with id {id} not found" });
      }

      return Ok(todo);
    }

    [HttpPost]
    public ActionResult<TodoItem> Create([FromBody] TodoItem newTodo)
    {
      if (string.IsNullOrEmpty(newTodo.Title))
      {
        return BadRequest(new { message = $"title is required" });
      }


      newTodo.Id = Todos.Count + 1;
      Todos.Add(newTodo);
      return CreatedAtAction(nameof(GetById), new { id = newTodo.Id }, newTodo);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] TodoItem updatedTodo)
    {
      var existingTodo = Todos.FirstOrDefault(todo => todo.Id == id);
      if (existingTodo == null)
      {
        return NotFound(new { message = $"todo item with id {id} not found" });
      }

      existingTodo.Title = updatedTodo.Title;
      return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
      var todo = Todos.FirstOrDefault(todo => todo.Id == id);
      if (todo == null)
      {
        return NotFound(new { message = $"todo item with id {id} not found" });
      }

      Todos.Remove(todo);
      return NoContent();
    }
  }
}
