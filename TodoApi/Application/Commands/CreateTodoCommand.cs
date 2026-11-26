namespace TodoApi.Application.Commands
{
  public record CreateTodoCommand(string Title, int UserId);
}
