using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

[Route("/api/v1/Card")]
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

    [HttpGet("From/{key}")]
    public async Task<IActionResult> FromCollection(string key) {
        return Ok(await _cardService.FromCollection(key));
    }

    [HttpPost("Relation")]
    public async Task<IActionResult> CreateRelation([FromBody] PostCardRelationWithType relation) {
        try {
            await _cardService.CreateRelation(relation.CardKey, relation.RelatedCardKey, relation.RelationType);
            return Created();
        } catch (CardNotFoundException e) {
            return NotFound(e.Message);
        } catch (RelationAlreadyExistsException e) {
            return BadRequest(e.Message);
        } catch (RelationWithSelfException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("Relation")]
    public async Task<IActionResult> DeleteRelation([FromBody] PostCardRelation relation) {
        try {
            await _cardService.DeleteRelation(relation.CardKey, relation.RelatedCardKey);
            return Ok();
        } catch (CardNotFoundException e) {
            return NotFound(e.Message);
        } catch (RelationNotFoundException e) {
            return NotFound(e.Message);
        } catch (RelationWithSelfException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch("Relation")]
    public async Task<IActionResult> EditRelationType([FromBody] PostCardRelationWithType relation) {
        try {
            await _cardService.EditRelationType(relation.CardKey, relation.RelatedCardKey, relation.RelationType);
            return Ok();
        } catch (CardNotFoundException e) {
            return NotFound(e.Message);
        } catch (RelationNotFoundException e) {
            return NotFound(e.Message);
        } catch (RelationWithSelfException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("Collections")]
    public async Task<IActionResult> GetCollections() {
        return Ok(await _cardService.GetCollections());
    }
}