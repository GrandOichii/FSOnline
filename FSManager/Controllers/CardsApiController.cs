using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

[Route("/api/v1/card")]
public class CardsApiController : ControllerBase {
    private readonly ICardService _cardService;

    public CardsApiController(ICardService cardService) {
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<IActionResult> All() {
        return Ok(await _cardService.All());
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> ByKey(string key) {
        try {
            return Ok(await _cardService.ByKey(key));
        } catch (CardNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key) {
        try {
            await _cardService.Delete(key);
            return Ok();
        } catch (CardNotFoundException ex) {
            return NotFound(ex.Message);
        } catch (FailedToDeleteCardException ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostCard card) {
        // TODO catch validation exceptions
        // try {
            var result = await _cardService.Create(card);
            return Ok(result);
        // } catch () {

        // }
    }
}