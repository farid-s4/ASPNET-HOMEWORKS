using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProfileCardMVC.Data;
using ProfileCardMVC.Models;

namespace ProfileCardMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private AppDbContext _database;

    public HomeController(ILogger<HomeController> logger,  AppDbContext database)
    {
        _logger = logger;
        _database = database;
    }

    public IActionResult Index()
    {
        var users =  _database.Users.ToList();
        if (users.FirstOrDefault() == null)
        {
            return View("/Views/Create/Create.cshtml");
        }
        return View(users);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}