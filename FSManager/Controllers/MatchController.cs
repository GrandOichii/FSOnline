using Microsoft.AspNetCore.Mvc;

namespace FSManager.Controllers;

[ApiController]
[Route("api/v1/Match")]
public class MatchController(IMatchService matchService) : ControllerBase {
    private readonly IMatchService _matchService = matchService;

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        return Ok(await _matchService.All());
    }

    // [HttpPost]
    // public async Task<IActionResult> Create(CreateMatchParams config) {
    //     try {
    //         var result = await _matchService.Create(config);
    //         return Ok(result);
    //     } catch (FailedToCreateMatchException e) {
    //         return BadRequest(e.Message);
    //     }
    // }

    [HttpGet("Create")]
    public async Task WebSocketCreate() {
        if (HttpContext.WebSockets.IsWebSocketRequest) {
            // var userId = this.ExtractClaim(ClaimTypes.NameIdentifier);
            // var userId = "";

            try {
                await _matchService.WebSocketCreate(HttpContext.WebSockets);
            } catch (Exception e) {
                // TODO handle
                System.Console.WriteLine(e);
                throw e;
            }

            // try {
            //     await _matchService.WSConnect(HttpContext.WebSockets, userId, matchId);
            // } catch (InvalidMatchIdException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest; 
            // } catch (MatchNotFoundException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            // } catch (MatchRefusedConnectionException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            // }
        } else {
            HttpContext.Response.StatusCode = 400;
        }
    }

    [HttpGet("{matchId}")]
    public async Task WebSocketConnect(string matchId) {
        if (HttpContext.WebSockets.IsWebSocketRequest) {
            // TODO handle exceptions
            await _matchService.WebSocketConnect(matchId, HttpContext.WebSockets);
        } else {
            HttpContext.Response.StatusCode = 400;
        }
    }
}