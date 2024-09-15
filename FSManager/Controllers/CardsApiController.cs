using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

[Route("/api/v1/Cards")]
public class CardsApiController : ControllerBase {
    private readonly ICardService _cardService;

    public CardsApiController(ICardService cardService) {
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<IActionResult> All(int page = 0) {
        return Ok(await _cardService.All(page));
    }

    [HttpGet("f")]
    public async Task<IActionResult> Filter(CardFilter filter, int page = 0) {
        return Ok(await _cardService.Filter(filter, page));
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
        try {
            var result = await _cardService.Create(card);
            return CreatedAtAction(nameof(ByKey), new { key = card.Key}, result);
        } catch (CardValidationException e) {
            return BadRequest(e.Message);
        } catch (CardKeyAlreadyExistsException e) {
            return BadRequest(e.Message);
        }

    }

    [HttpPatch("{key}")]
    public async Task<IActionResult> Edit([FromBody] PostCard card, string key) {
        try {
            var result = await _cardService.Edit(key, card);
            return Ok(result);
        } catch (CardValidationException e) {
            return BadRequest(e.Message);
        } catch (CardNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("From/{key}")]
    public async Task<IActionResult> FromCollection(string key, [FromQuery] int page = 0) {
        return Ok(await _cardService.FromCollection(key, page));
    }

    [HttpGet("Relation/{key1}/{key2}")]
    public async Task<IActionResult> GetRelation(string key1, string key2) {
        try {
            var result = await _cardService.GetRelation(key1, key2);
            return Ok(result);
        } catch (CardNotFoundException e) {
            return NotFound(e.Message);
        } catch (RelationNotFoundException e) {
            return NotFound(e.Message);
        }
    }

    [HttpPost("Relation")]
    public async Task<IActionResult> CreateRelation([FromBody] PostCardRelationWithType relation) {
        try {
            await _cardService.CreateRelation(relation.CardKey, relation.RelatedCardKey, relation.RelationType);
            // return CreatedAtAction(typeof());
            return StatusCode(201);
        } catch (CardNotFoundException e) {
            return NotFound(e.Message);
        } catch (RelationAlreadyExistsException e) {
            return BadRequest(e.Message);
        } catch (RelationWithSelfException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("Relation/{key1}/{key2}")]
    public async Task<IActionResult> DeleteRelation(string key1, string key2) {
        try {
            await _cardService.DeleteRelation(key1, key2);
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