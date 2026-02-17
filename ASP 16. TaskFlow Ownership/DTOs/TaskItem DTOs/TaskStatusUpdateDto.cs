using TaskStatus = ASP_16._TaskFlow_Ownership.Models.TaskStatus;

namespace ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;

public class TaskStatusUpdateDto
{
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
}
