using Microsoft.AspNetCore.Http.HttpResults;

namespace TodoApi.Models
{
  public class TodoItem
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? DueDate { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TodoItem()
    {

    }

    public TodoItem(string title, PriorityLevel priority = PriorityLevel.Medium)
    {
      Title = title;
      Priority = priority;
      CreatedAt = DateTime.UtcNow;
    }

    public bool isOverdue()
    {
      if (!DueDate.HasValue || IsCompleted)
      {
        return false;
      }

      return DateTime.UtcNow > DueDate.Value;
    }

    // public void MarķAsComplete()
    // {
    //   IsCompleted = true;
    // }
    public void MarkAsComplete() => IsCompleted = true;
  }
}
