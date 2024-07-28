using Microsoft.AspNetCore.Mvc;

namespace FSMatchManager.Controllers;

[ApiController]
[Route("v1/Match")]
public class MatchController(IMatchService matchService) : Controller {
    private readonly IMatchService _matchService = matchService;

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        return Ok(await _matchService.All());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMatch config) {
        try {
            var result = await _matchService.Create(config);
            return Ok(result);
        } catch (FailedToCreateMatchException e) {
            return BadRequest(e.Message);
        }
    }
}