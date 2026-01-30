using Microsoft.AspNetCore.Mvc;
using ProfileCardMVC.Data;
using ProfileCardMVC.Models;

namespace ProfileCardMVC.Controllers;

public class CreateController : Controller
{
    private AppDbContext _context;

    public CreateController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }
}