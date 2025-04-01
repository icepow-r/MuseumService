namespace MuseumService.Models.Services;

// ScoresController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ScoresController : ControllerBase
{
    private readonly ScoreService _scoreService;
    
    public ScoresController(ScoreService scoreService)
    {
        _scoreService = scoreService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var scores = await _scoreService.GetAllScores();
        return Ok(scores);
    }
    
    [HttpGet("game/{gameId}")]
    public async Task<IActionResult> GetByGame(int gameId)
    {
        var scores = await _scoreService.GetScoresByGame(gameId);
        return Ok(scores);
    }
    
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateScoreDto dto)
    {
        var score = await _scoreService.AddScore(dto);
        return CreatedAtAction(nameof(GetAll), score);
    }
}