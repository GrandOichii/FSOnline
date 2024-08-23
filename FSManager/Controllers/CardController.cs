using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

public class CardController : Controller {
    private readonly ICardRepository _cards;

    public CardController(ICardRepository cards) {
        _cards = cards;
    }

    
}