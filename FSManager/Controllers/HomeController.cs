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

    public async Task<IActionResult> Cards() {
        var cards = (await _cards.All()).ToList();
        return View(cards);
    }

    public IActionResult DeleteCard(string cardKey) {
        // TODO
        // var card = _cards.Cards.FirstOrDefault(c => c.Key == cardKey);
        // if (card is null) {
        //     // TODO
        //     throw new Exception($"No card with key {cardKey}");
        // }

        // _cards.Cards.Remove(card);
        // _cards.SaveChanges();
        
        return RedirectToAction("Cards");
    }

    public IActionResult CreateCard() {
        return View();
    }

    public async Task<IActionResult> CreateCardForm(PostCard card) { // TODO change name
        // TODO catch exceptions
        await _cards.Create(card);

        return RedirectToAction("Cards");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
