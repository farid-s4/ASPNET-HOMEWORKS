using ASP_16._TaskFlow_Ownership.Common;
using ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;
using ASP_16._TaskFlow_Ownership.Models;

namespace ASP_16._TaskFlow_Ownership.Services.Interfaces;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
    Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams);
    Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
    Task<TaskItem?> GetTaskEntityAsync(int id);
    Task<TaskItemResponseDto?> GetByIdAsync(int id);
    Task<TaskItemResponseDto> CreateAsync(CreateTaskItemDto createTask);
    Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemDto updateTask);
    Task<TaskItemResponseDto?> UpdateStatusAsync(int id, TaskStatusUpdateDto taskStatus);
    Task<bool> DeleteAsync(int id);
}
