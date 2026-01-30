using Microsoft.AspNetCore.Mvc;
using ProfileCardMVC.Data;
using ProfileCardMVC.Models;

namespace ProfileCardMVC.Controllers;

public class ProjectsController : Controller
{
    private AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            int savedUserId = project.UserId; 
        
            _context.Projects.Remove(project); 
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Details", "Details", new { id = savedUserId });
        }
    
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public IActionResult CreateProjects(int userId)
    {
        var project = new Project
        {
            UserId = userId
        };
        return View(project);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProjects(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Details", new { id = project.UserId });
    }
}