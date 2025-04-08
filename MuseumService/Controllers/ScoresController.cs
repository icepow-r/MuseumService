using Microsoft.AspNetCore.Mvc;
using MuseumService.Models;
using MuseumService.Models.Services;

namespace MuseumService.Controllers;

/// <summary>
/// Контроллер для управления результатами игр
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScoresController : ControllerBase
{
    private readonly ScoreService _scoreService;
    
    public ScoresController(ScoreService scoreService)
    {
        _scoreService = scoreService;
    }
    
    /// <summary>
    /// Получение всех результатов
    /// </summary>
    /// <returns>Список результатов</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Score>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var scores = await _scoreService.GetAllScores();
        return Ok(scores);
    }
    
    /// <summary>
    /// Получение результатов по идентификатору игры
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <returns>Список результатов для указанной игры</returns>
    [HttpGet("game/{gameId}")]
    [ProducesResponseType(typeof(IEnumerable<Score>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGame(int gameId)
    {
        var scores = await _scoreService.GetScoresByGame(gameId);
        return Ok(scores);
    }
    
    /// <summary>
    /// Добавление нового результата
    /// </summary>
    /// <param name="dto">Данные для создания результата</param>
    /// <returns>Созданный результат</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Score), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateScoreDto dto)
    {
        var score = await _scoreService.AddScore(dto);
        return CreatedAtAction(nameof(GetAll), score);
    }
}