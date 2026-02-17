using ASP_16._TaskFlow_Ownership.Models;
using System.ComponentModel.DataAnnotations;

namespace ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;

public class CreateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }

}
