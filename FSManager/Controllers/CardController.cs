using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

public class CardController : Controller {
    private readonly CardsContext _cards;

    public CardController(CardsContext cards) {
        _cards = cards;
    }

    
}