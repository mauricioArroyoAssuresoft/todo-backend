using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  // [Authorize]
  public class TodosController : ControllerBase
  {
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodosController> _logger;

    public TodosController(
      ITodoRepository todoRepository,
      ILogger<TodosController> logger
    )
    {
      _todoRepository = todoRepository;
      _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetAll()
    {
      var todos = await _todoRepository.GetAllAsync();
      var todosDtos = todos.Select(todo => new TodoResponseDto(todo.Id, todo.Title, todo.Description, todo.IsCompleted, (int)todo.Priority));

      return Ok(todosDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
      _logger.LogInformation("fetching todo item with ID {id}", id);
      var todo = await _todoRepository.GetByIdAsync(id);
      if (todo is null)
      {
        _logger.LogWarning("todo item with ID {id} was not found", id);
        return NotFound(new { message = $"todo item with id {id} not found" });
      }

      return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] TodoItem newTodo)
    {
      var entity = new TodoItem
      {
        Title = newTodo.Title,
        Description = newTodo.Description,
        Priority = newTodo.Priority,
        IsCompleted = false
      };
      var created = await _todoRepository.CreateAsync(entity);
      var responseDto = new TodoResponseDto(created.Id, created.Title, created.Description, created.IsCompleted, (int)created.Priority);

      return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);

    }

    // [HttpPut("{id:int}")]
    // public IActionResult Update(int id, [FromBody] TodoItem updatedTodo)
    // {
    //   var existingTodo = Todos.FirstOrDefault(todo => todo.Id == id);
    //   if (existingTodo == null)
    //   {
    //     return NotFound(new { message = $"todo item with id {id} not found" });
    //   }

    //   existingTodo.Title = updatedTodo.Title;
    //   return NoContent();
    // }

    // [HttpDelete("{id:int}")]
    // public IActionResult Delete(int id)
    // {
    //   var todo = Todos.FirstOrDefault(todo => todo.Id == id);
    //   if (todo == null)
    //   {
    //     return NotFound(new { message = $"todo item with id {id} not found" });
    //   }

    //   Todos.Remove(todo);
    //   return NoContent();
    // }

    // [HttpGet("pending")]
    // public async Task<ActionResult<IEnumerable<TodoItem>>> GetPendingTodos()
    // {
    //   var pendingTodos = await _context.Todos
    //     .Where(todo => !todo.IsCompleted)
    //     .OrderByDescending(todo => todo.Priority)
    //     .ToListAsync();

    //   return Ok(pendingTodos);
    // }

    // [HttpGet("search")]
    // public async Task<ActionResult<IEnumerable<TodoItem>>> Search([FromQuery] string keyword)
    // {
    //   var results = await _context.Todos
    //     .Where(todos => todos.Title.Contains(keyword))
    //     .AsNoTracking()
    //     .ToListAsync();

    //   return Ok(results);
    // }

    // [HttpGet("stats")]
    // public async Task<IActionResult> GetStats()
    // {
    //   var total = await _context.Todos.CountAsync();
    //   var completed = await _context.Todos.CountAsync(todo => todo.IsCompleted);

    //   return Ok(new { Total = total, Completed = completed, Remaining = total - completed });
    // }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
      _logger.LogInformation("LOGGER: checking api health status");
      return Ok(new { status = "asp.net core web api is running" });
    }


    [HttpGet("profile")]
    public IActionResult GetUserProfile()
    {
      return Ok(new { User = User.Identity?.Name });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
      return NoContent();
    }

    [HttpGet("crash-test")]
    public IActionResult CrashTest()
    {
      throw new InvalidOperationException("simulated databse failure!");
    }

  }
}
