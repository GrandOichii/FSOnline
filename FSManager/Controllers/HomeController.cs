using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FSManager.Models;

namespace FSManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICardService _cards;

    public HomeController(ILogger<HomeController> logger, ICardService cards)
    {
        _logger = logger;
        _cards = cards;
    }

    public IActionResult Index()
    {
        return View();
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
