using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileCardMVC.Data;
using ProfileCardMVC.Models;

namespace ProfileCardMVC.Controllers;

public class DetailsController : Controller
{
    private AppDbContext _context;

    public DetailsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var user = _context.Users
            .Include(x => x.Skills)
            .Include(x => x.Projects)
            .FirstOrDefault(x => x.Id == id);
        
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }
}