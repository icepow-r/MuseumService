using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumService.Models;
using MuseumService.Models.Services;

namespace MuseumService.Controllers;

/// <summary>
/// Контроллер для управления экспонатами музея
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExhibitsController : ControllerBase
{
    private readonly ExhibitService _exhibitService;
    
    public ExhibitsController(ExhibitService exhibitService)
    {
        _exhibitService = exhibitService;
    }
    
    /// <summary>
    /// Получение списка всех экспонатов
    /// </summary>
    /// <returns>Список экспонатов</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Exhibit>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var exhibits = await _exhibitService.GetAllExhibits();
        return Ok(exhibits);
    }
    
    /// <summary>
    /// Получение экспоната по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор экспоната</param>
    /// <returns>Экспонат</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Exhibit), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var exhibit = await _exhibitService.GetExhibitById(id);
        if (exhibit == null)
            return NotFound();
            
        return Ok(exhibit);
    }
    
    /// <summary>
    /// Создание нового экспоната
    /// </summary>
    /// <param name="dto">Данные для создания экспоната</param>
    /// <returns>Созданный экспонат</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Exhibit), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateExhibitDto dto)
    {
        var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var exhibit = await _exhibitService.CreateExhibit(dto, employeeId);
        return CreatedAtAction(nameof(GetById), new { id = exhibit.ExhibitId }, exhibit);
    }
    
    /// <summary>
    /// Обновление информации об экспонате
    /// </summary>
    /// <param name="id">Идентификатор экспоната</param>
    /// <param name="dto">Данные для обновления</param>
    /// <returns>Обновленный экспонат</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Exhibit), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExhibitDto dto)
    {
        var exhibit = await _exhibitService.UpdateExhibit(id, dto);
        if (exhibit == null)
            return NotFound();
            
        return Ok(exhibit);
    }
    
    /// <summary>
    /// Удаление экспоната
    /// </summary>
    /// <param name="id">Идентификатор экспоната</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _exhibitService.DeleteExhibit(id);
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}