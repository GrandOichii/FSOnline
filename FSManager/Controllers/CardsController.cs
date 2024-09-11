using FSCore.Matches.Actions;
using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

public class CardsPage {
    public required List<GetCard> Cards { get; set; }
    public required int Page { get; set; }
}

public class CardsController : Controller {
    private readonly ICardService _cards;

    public CardsController(ICardService cards) {
        _cards = cards;
    }

    public async Task<IActionResult> All(int page = 0) {
        var cards = (await _cards.All(page)).ToList();
        return View(new CardsPage {
            Cards = cards,
            Page = page,
        });
    }

    public async Task<IActionResult> ByKey(string key) {
        var card = await _cards.ByKey(key);
        return View(card);
    }

    public async Task<IActionResult> Delete(string key) {
        // TODO catch exceptions
        await _cards.Delete(key);
        
        return RedirectToAction("All");
    }

    public IActionResult CreateForm() {
        return View();
    }

    public async Task<IActionResult> Create(PostCard card) { // TODO change name
        // TODO catch exceptions
        await _cards.Create(card);

        return RedirectToAction("All");
    }

    
}