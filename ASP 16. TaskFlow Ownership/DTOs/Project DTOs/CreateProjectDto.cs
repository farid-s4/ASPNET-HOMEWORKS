using ASP_16._TaskFlow_Ownership.Models;

namespace ASP_16._TaskFlow_Ownership.DTOs;
/// <summary>
/// DTO for create project. Uses for POST requestes
/// </summary>
public class CreateProjectDto
{
    /// <summary>
    /// Project name
    /// </summary>
    /// <example>My new project</example>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Project Description
    /// </summary>
    /// <example>Description for my project</example>
    public string Description { get; set; } = string.Empty;
}
    
