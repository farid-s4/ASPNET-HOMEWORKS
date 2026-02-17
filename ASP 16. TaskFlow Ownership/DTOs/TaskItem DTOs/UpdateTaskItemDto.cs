using ASP_16._TaskFlow_Ownership.Models;

namespace ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;

public class UpdateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public Models.TaskStatus Status { get; set; } = Models.TaskStatus.ToDo;
}
