using FSCore.Matches.Actions;
using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

public class CardsController : Controller {
    private readonly ICardService _cards;

    public CardsController(ICardService cards) {
        _cards = cards;
    }

    public async Task<IActionResult> All() {
        var cards = (await _cards.All(0)).ToList();
        return View(cards);
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