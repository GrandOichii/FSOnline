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

    public IActionResult Delete(string cardKey) {
        // TODO
        // var card = _cards.Cards.FirstOrDefault(c => c.Key == cardKey);
        // if (card is null) {
        //     // TODO
        //     throw new Exception($"No card with key {cardKey}");
        // }

        // _cards.Cards.Remove(card);
        // _cards.SaveChanges();
        
        return RedirectToAction("All");
    }

    public IActionResult Create() {
        return View();
    }

    public async Task<IActionResult> CreateCardForm(PostCard card) { // TODO change name
        // TODO catch exceptions
        await _cards.Create(card);

        return RedirectToAction("All");
    }

}