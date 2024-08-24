using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

public class CardsController : Controller {
    private readonly ICardService _cards;

    public CardsController(ICardService cards) {
        _cards = cards;
    }

    public async Task<IActionResult> All() {
        var cards = (await _cards.All()).ToList();
        return View(cards);
    }

    public async Task<IActionResult> Delete(string cardKey) {
        // TODO catch exceptions
        await _cards.Delete(cardKey);
        
        return RedirectToAction("All");
    }

    public IActionResult Create() {
        return View();
    }

    public async Task<IActionResult> CreateForm(PostCard card) { // TODO change name
        // TODO catch exceptions
        await _cards.Create(card);

        return RedirectToAction("All");
    }

}