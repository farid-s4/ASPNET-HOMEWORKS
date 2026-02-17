using ASP_16._TaskFlow_Ownership.Common;
using ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;
using ASP_16._TaskFlow_Ownership.Models;
using ASP_16._TaskFlow_Ownership.Services;
using ASP_16._TaskFlow_Ownership.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP_16._TaskFlow_Ownership.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy ="UserOrAbove")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    public TaskItemsController(
        ITaskItemService taskItemService, 
        IProjectService projectService, 
        IAuthorizationService authorizationService)
    {
        _taskItemService = taskItemService;
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Retrieves all task items.
    /// </summary>
    /// <returns>List of all task items.</returns>
    /// <response code="200">Returns the list of task items.</response>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetAll()
    {
        var tasks = await _taskItemService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(tasks, "Returns the list of task items."));
    }

    /// <summary>
    /// Retrieves a paged, sorted, and filtered list of task items based on query parameters.
    /// </summary>
    /// <param name="queryParams">
    /// Query parameters for pagination, sorting, filtering, and searching task items.
    /// </param>
    /// <returns>Paged result containing task items matching the specified criteria.</returns>
    /// <response code="200">Returns the paged list of task items.</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TaskItemResponseDto>>>> GetPaged([FromQuery] TaskItemQueryParams queryParams)
    {
        var result = await _taskItemService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<TaskItemResponseDto>>.SuccessResponse(result, "Returns the list of task successfully."));
    }

    /// <summary>
    /// Retrieves a task item by its identifier.
    /// </summary>
    /// <param name="id">Task item identifier.</param>
    /// <returns>The task item with the specified ID.</returns>
    /// <response code="200">Returns the task item if found.</response>
    /// <response code="404">If the task item is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> GetById(int id)
    {
        var task = await _taskItemService.GetByIdAsync(id);
        
        if (task is null)
            return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task with ID {id} not found"));

        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);

        if (project is null)
            return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authResult.Succeeded)
            return Forbid();

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item found."));
    }

    /// <summary>
    /// Retrieves all task items for a specific project.
    /// </summary>
    /// <param name="projectId">Project identifier.</param>
    /// <returns>List of task items for the specified project.</returns>
    /// <response code="200">Returns the list of task items for the project.</response>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetByProjectId(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authResult.Succeeded)
            return Forbid();

        var tasks = await _taskItemService.GetByProjectIdAsync(projectId);
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(tasks, "Returns the list of task items for the project."));
    }

    /// <summary>
    /// Creates a new task item.
    /// </summary>
    /// <param name="createTask">Task item data to create.</param>
    /// <returns>The created task item.</returns>
    /// <response code="201">Returns the newly created task item.</response>
    /// <response code="400">If the model is invalid or the request is incorrect.</response>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Create([FromBody] CreateTaskItemDto createTask)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Invalid model state.", default));

        var project = await _projectService.GetProjectEntityAsync(createTask.ProjectId);

        if (project is null)
            return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var task = await _taskItemService.CreateAsync(createTask);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item created successfully."));

        //try
        //{
        //    var task = await _taskItemService.CreateAsync(createTask);
        //    return CreatedAtAction(nameof(GetById), new { id = task.Id }, ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item created successfully."));
        //}
        //catch (ArgumentException ex)
        //{
        //    return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse(ex.Message));
        //}
    }

    /// <summary>
    /// Updates an existing task item.
    /// </summary>
    /// <param name="id">Task item identifier.</param>
    /// <param name="updateTask">Updated task item data.</param>
    /// <returns>The updated task item.</returns>
    /// <response code="200">Returns the updated task item.</response>
    /// <response code="400">If the model is invalid.</response>
    /// <response code="404">If the task item is not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Update(int id, [FromBody] UpdateTaskItemDto updateTask)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Invalid model state.", default));
        var task = await _taskItemService.GetTaskEntityAsync(id);

        if (task is null)
            return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task with ID {id} not found"));

        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);

        if (project is null)
            return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var updatedtask = await _taskItemService.UpdateAsync(id, updateTask);

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(updatedtask!, "Task item updated successfully."));
    }

    /// <summary>
    /// Deletes a task item by its identifier.
    /// </summary>
    /// <param name="id">Task item identifier.</param>
    /// <returns>No content if deleted.</returns>
    /// <response code="200">Task item deleted successfully.</response>
    /// <response code="404">If the task item is not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _taskItemService.GetTaskEntityAsync(id);

        if (task is null)
            return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task with ID {id} not found"));

        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);

        if (project is null)
            return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var isDeleted = await _taskItemService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound(ApiResponse<object>.ErrorResponse($"Task with ID {id} not found"));

        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> UpdateStatus(int id, [FromBody] TaskStatusUpdateDto taskStatusUpdate)
    {
        var task = await _taskItemService.GetTaskEntityAsync(id);

        if (task is null)
            return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task with ID {id} not found"));

        var authResult = await _authorizationService.AuthorizeAsync(User, task, "TaskStatusChange");

        if (!authResult.Succeeded)
            return Forbid();

        var updated = await _taskItemService.UpdateStatusAsync(id, taskStatusUpdate);

        if (updated is null)
            return BadRequest();

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(updated, "Status updated"));

    }
}
