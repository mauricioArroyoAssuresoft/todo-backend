using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs
{
  public record CreateTodoDto(
    [Required(ErrorMessage = "Title is mandatory.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    string Title,

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 cahracters")]
    string? Description,
    
    [Range(1, 5, ErrorMessage = "Priority must be between 1 (low) and 5 (high)")]
    int Priority);
}
