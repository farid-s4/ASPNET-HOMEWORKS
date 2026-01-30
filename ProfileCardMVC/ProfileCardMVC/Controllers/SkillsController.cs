using Microsoft.AspNetCore.Mvc;
using ProfileCardMVC.Data;
using ProfileCardMVC.Models;

namespace ProfileCardMVC.Controllers;

public class SkillsController : Controller
{
    private AppDbContext _context;

    public SkillsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill != null)
        {
            int savedUserId = skill.UserId; 
        
            _context.Skills.Remove(skill); 
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Details", "Details", new { id = savedUserId });
        }
    
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public IActionResult CreateSkills(int userId)
    {
        var skill = new Skill
        {
            UserId = userId
        };
        return View(skill);
    }
    [HttpPost]
    public async Task<IActionResult> CreateSkills(Skill skill)
    {
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Details", new { id = skill.UserId });
    }
}