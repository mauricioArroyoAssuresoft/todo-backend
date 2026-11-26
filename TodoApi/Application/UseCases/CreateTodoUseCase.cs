using TodoApi.Application.Commands;
using TodoApi.Application.DTOs;
using TodoApi.Application.Interfaces;
using TodoApi.Domain.Entities;

namespace TodoApi.Application.UseCases
{
  public class CreateTodoUseCase
  {
    private readonly ITodoRepository _repository;
    public CreateTodoUseCase(ITodoRepository repository)
    {
      _repository = repository;
    }

    public async Task<TodoDto> ExecuteAsync(CreateTodoCommand createTodoCommand)
    {
      var todo = new TodoItem(createTodoCommand.Title);
      var saved = await _repository.AddAsync(todo);
      return new TodoDto(saved.Id, saved.Title, saved.IsCompleted);
    }
  }
}
