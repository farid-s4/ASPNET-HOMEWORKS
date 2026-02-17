using ASP_16._TaskFlow_Ownership.Common;
using ASP_16._TaskFlow_Ownership.DTOs;
using ASP_16._TaskFlow_Ownership.DTOs.Project_DTOs;
using ASP_16._TaskFlow_Ownership.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_16._TaskFlow_Ownership.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private IList<string> UserRoles => User.Claims
                                            .Where(c => c.Type == ClaimTypes.Role)
                                            .Select(c => c.Value)
                                            .ToList();

    public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <returns>List of all projects.</returns>
    /// <response code="200">Returns the list of projects successfully.</response>
    [HttpGet] 
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponseDto>>>> GetAll()
    {
        var projects = await _projectService.GetAllForUserAsync(UserId!, UserRoles);
        return Ok(ApiResponse<IEnumerable<ProjectResponseDto>>.SuccessResponse(projects, "Returns the list of projects successfully."));
    }

    /// <summary>
    /// Retrieves a project by its specific identifier.
    /// </summary>
    /// <param name="id">Project identifier.</param>
    /// <returns>The project with the specified ID.</returns>
    /// <response code="200">Returns the project if found.</response>
    /// <response code="404">If the project is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);
        if (project is null)
            return NotFound(ApiResponse<ProjectResponseDto>.ErrorResponse($"Project with ID {id} not found"));

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var responseResult = await _projectService.GetByIdAsync(id);        

        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(responseResult!, "Project found."));
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="createProjectDto">Project data to create.</param>
    /// <returns>The created project.</returns>
    /// <response code="201">Returns the newly created project.</response>
    /// <response code="400">If the model is invalid.</response>
    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create([FromBody] CreateProjectDto createProjectDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ProjectResponseDto>.ErrorResponse("Invalid model state."));

        var createdProject = await _projectService.CreateAsync(createProjectDto, UserId!);
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            ApiResponse<ProjectResponseDto>.SuccessResponse(createdProject, "Project created successfully."));
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">Project identifier.</param>
    /// <param name="updateProjectDto">Updated project data.</param>
    /// <returns>The updated project.</returns>
    /// <response code="200">Returns the updated project.</response>
    /// <response code="400">If the model is invalid.</response>
    /// <response code="404">If the project is not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ProjectResponseDto>.ErrorResponse("Invalid model state."));

        var project = await _projectService.GetProjectEntityAsync(id);
        if (project is null)
            return NotFound(ApiResponse<ProjectResponseDto>.ErrorResponse($"Project with ID {id} not found"));

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var updatedProject = await _projectService.UpdateAsync(id, updateProjectDto);

        if (updatedProject is null)
            return NotFound(ApiResponse<ProjectResponseDto>.ErrorResponse($"Project with ID {id} not found"));

        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(updatedProject, "Project updated successfully."));
    }

    /// <summary>
    /// Deletes a project by its identifier.
    /// </summary>
    /// <param name="id">Project identifier.</param>
    /// <returns>No content if deleted.</returns>
    /// <response code="204">Project deleted successfully.</response>
    /// <response code="404">If the project is not found.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {

        var project = await _projectService.GetProjectEntityAsync(id);
        if (project is null)
            return NotFound(ApiResponse<ProjectResponseDto>.ErrorResponse($"Project with ID {id} not found"));

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var isDeleted = await _projectService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound(ApiResponse<object>.ErrorResponse($"Project with ID {id} not found"));

        return NoContent();
    }

    [HttpGet("{projectId}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMemberResponse>>>> GetMembers(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null) 
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var members = await _projectService.GetMembersAsync(projectId);

        return Ok(ApiResponse<IEnumerable<ProjectMemberResponse>>.SuccessResponse(members, "Project memebers retrived successfully"));

    }

    [HttpGet("{projectId}/available-users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AvailableUserDto>>>> GetAvailableUsersToAdd(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var users = await _projectService.GetAvailableUsersToAddAsync(projectId);

        return Ok(ApiResponse<IEnumerable<AvailableUserDto>>.SuccessResponse(users, "Project memebers retrived successfully"));

    }

    [HttpPost("{projectId}/members")]
    public async Task<IActionResult> AddMember(int projectId, [FromBody] AddProjectMemberDto dto)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var userIdOrEmail = dto.UserId ?? dto.Email?.Trim();

        if (string.IsNullOrEmpty(userIdOrEmail))
            return BadRequest();

        var added = await _projectService.AddMemberAsync(projectId, userIdOrEmail);

        if (!added)
            return BadRequest("User not found or already added");

        return NoContent();
    }

    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(int projectId, string userId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var removed = await _projectService.RemoveMemberAsync(projectId, userId);

        if (!removed) 
            return NotFound();

        return NoContent();
    }

}
