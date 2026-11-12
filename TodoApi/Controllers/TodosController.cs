using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

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
    private readonly ITodoRepository _todoRepository;
    private readonly TodoDbContext _context;


    public TodosController(ILogger<TodosController> logger, ITodoRepository todoRepository, TodoDbContext context)
    {
      _logger = logger;
      _todoRepository = todoRepository;
      _context = context;
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
      var allTodos = _todoRepository.GetAll();
      return Ok(allTodos);
    }

    [HttpGet("db")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAllFromDb()
    {
      var todosFromDb = await _context.Todos.ToListAsync();
      return Ok(todosFromDb);
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

    [HttpPost("db")]
    public async Task<ActionResult<TodoItem>> CreateInDb(TodoItem todo)
    {
      _context.Todos.Add(todo);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetAllFromDb), new { id = todo.Id }, todo);
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
