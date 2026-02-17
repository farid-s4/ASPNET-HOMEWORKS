using Microsoft.AspNetCore.Authorization;

namespace ASP_16._TaskFlow_Ownership.Authorization;

public class ProjectOwnerOrAdminRequirement
    : IAuthorizationRequirement
{
}
