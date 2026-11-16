using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
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
    public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetAllFromDb()
    {
      var todosFromDb = await _context.Todos.ToListAsync();
      var todosDtos = todosFromDb.Select(todo => new TodoResponseDto(todo.Id, todo.Title, todo.Description, todo.IsCompleted, (int)todo.Priority));
      return Ok(todosDtos);
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
    public async Task<ActionResult<TodoResponseDto>> CreateInDb(CreateTodoDto dtoTodo)
    {
      var entity = new TodoItem
      {
        Title = dtoTodo.Title,
        Description = dtoTodo.Description,
        Priority = (PriorityLevel)dtoTodo.Priority,
        IsCompleted = false
      };
      _context.Todos.Add(entity);
      await _context.SaveChangesAsync();

      var responseTodo = new TodoResponseDto(entity.Id, entity.Title, entity.Description, entity.IsCompleted, (int) entity.Priority);
      return CreatedAtAction(nameof(GetAllFromDb), new { id = responseTodo.Id }, responseTodo);
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

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetPendingTodos()
    {
      var pendingTodos = await _context.Todos
        .Where(todo => !todo.IsCompleted)
        .OrderByDescending(todo => todo.Priority)
        .ToListAsync();

      return Ok(pendingTodos);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> Search([FromQuery] string keyword)
    {
      var results = await _context.Todos
        .Where(todos => todos.Title.Contains(keyword))
        .AsNoTracking()
        .ToListAsync();

      return Ok(results);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
      var total = await _context.Todos.CountAsync();
      var completed = await _context.Todos.CountAsync(todo => todo.IsCompleted);

      return Ok(new { Total = total, Completed = completed, Remaining = total - completed });
    }

  }
}
