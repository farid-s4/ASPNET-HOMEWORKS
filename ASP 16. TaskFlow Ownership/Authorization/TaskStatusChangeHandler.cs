using ASP_16._TaskFlow_Ownership.Data;
using ASP_16._TaskFlow_Ownership.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_16._TaskFlow_Ownership.Authorization;

public class TaskStatusChangeHandler
    : AuthorizationHandler<TaskStatusChangeRequrement, TaskItem>
{

    private readonly TaskFlowDbContext _context;

    public TaskStatusChangeHandler(TaskFlowDbContext context)
    {
        _context = context;
    }

    protected async override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        TaskStatusChangeRequrement requirement, 
        TaskItem resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return;

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var project = await _context.Projects
                                .FirstOrDefaultAsync(p => p.Id == resource.ProjectId);

        if (project is null) 
            return;

        if (context.User.IsInRole("Manager") && project.OwnerId == userId)
        {
            context.Succeed(requirement);
            return;
        }

        var isMember = await _context.ProjectMembers
                                .AnyAsync(m => m.ProjectId == resource.ProjectId && m.UserId == userId);

        if (isMember)
            context.Succeed(requirement);
    }
}
