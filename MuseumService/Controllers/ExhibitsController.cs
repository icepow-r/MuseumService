using System.Security.Claims;
using MuseumService.Models;
using MuseumService.Models.Services;

namespace MuseumService.Controllers;

// ExhibitsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var exhibits = await _exhibitService.GetAllExhibits();
        return Ok(exhibits);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var exhibit = await _exhibitService.GetExhibitById(id);
        if (exhibit == null)
            return NotFound();
            
        return Ok(exhibit);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExhibitDto dto)
    {
        var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var exhibit = await _exhibitService.CreateExhibit(dto, employeeId);
        return CreatedAtAction(nameof(GetById), new { id = exhibit.ExhibitId }, exhibit);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExhibitDto dto)
    {
        var exhibit = await _exhibitService.UpdateExhibit(id, dto);
        if (exhibit == null)
            return NotFound();
            
        return Ok(exhibit);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _exhibitService.DeleteExhibit(id);
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}