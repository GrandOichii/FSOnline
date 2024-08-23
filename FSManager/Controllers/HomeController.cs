using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FSManager.Models;

namespace FSManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CardsContext _cards;

    public HomeController(ILogger<HomeController> logger, CardsContext cards)
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

    public IActionResult Cards() {
        var cards = _cards.Cards.ToList();
        return View(cards);
    }

    public IActionResult DeleteCard(string cardKey) {
        var card = _cards.Cards.FirstOrDefault(c => c.Key == cardKey);
        if (card is null) {
            // TODO
            throw new Exception($"No card with key {cardKey}");
        }

        _cards.Cards.Remove(card);
        _cards.SaveChanges();
        
        return RedirectToAction("Cards");
    }

    public IActionResult CreateCard() {
        return View();
    }

    public IActionResult CreateCardForm(CardModel card) { // TODO change name
        System.Console.WriteLine($"Creating card {card.Key}");

        _cards.Cards.Add(card);
        _cards.SaveChanges();

        return RedirectToAction("Cards");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
