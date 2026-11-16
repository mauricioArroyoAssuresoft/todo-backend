namespace TodoApi.DTOs
{
  public record TodoResponseDto(int Id, string Title, string? Description, bool IsCompleted, int Priority);
}
