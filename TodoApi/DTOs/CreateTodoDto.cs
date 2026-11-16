namespace TodoApi.DTOs
{
  public record CreateTodoDto(string Title, string? Description, int Priority);
}
