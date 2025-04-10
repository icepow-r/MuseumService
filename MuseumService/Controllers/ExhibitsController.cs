using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumService.Models;
using MuseumService.Models.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _context;
    
    public ExhibitsController(ExhibitService exhibitService, IWebHostEnvironment environment, AppDbContext context)
    {
        _exhibitService = exhibitService;
        _environment = environment;
        _context = context;
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
    public async Task<IActionResult> Create([FromBody] JsonElement requestData)
    {
        try
        {
            int employeeId;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!int.TryParse(userIdClaim, out employeeId))
            {
                var userName = userIdClaim;
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == userName);
                
                if (employee == null)
                {
                    return BadRequest(new { error = "Не удалось определить сотрудника" });
                }
                
                employeeId = employee.EmployeeId;
            }
            
            var title = requestData.GetProperty("title").GetString();
            var description = requestData.GetProperty("desc").GetString();
            var loadOrder = requestData.GetProperty("loadOrder").GetInt32();
            
            var dto = new CreateExhibitDto
            {
                Title = title,
                Description = description,
                LoadOrder = loadOrder,
                Images = new List<ExhibitImageDto>()
            };
            
            if (requestData.TryGetProperty("images", out var imagesElement) && imagesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var imageElement in imagesElement.EnumerateArray())
                {
                    var imageExt = imageElement.GetProperty("imageExt").GetString();
                    var altText = imageElement.TryGetProperty("altText", out var altTextElement) ? 
                                  altTextElement.GetString() : title;
                    
                    var byteArrayElement = imageElement.GetProperty("byteArray");
                    byte[] byteArray;
                    
                    if (byteArrayElement.ValueKind == JsonValueKind.Array)
                    {
                        var bytes = new List<byte>();
                        foreach (var b in byteArrayElement.EnumerateArray())
                        {
                            bytes.Add(b.GetByte());
                        }
                        byteArray = bytes.ToArray();
                    }
                    else if (byteArrayElement.ValueKind == JsonValueKind.String)
                    {
                        var base64String = byteArrayElement.GetString();
                        byteArray = Convert.FromBase64String(base64String);
                    }
                    else
                    {
                        throw new InvalidOperationException("Неподдерживаемый формат байтового массива");
                    }
                    
                    dto.Images.Add(new ExhibitImageDto
                    {
                        ImageExt = imageExt,
                        AltText = altText,
                        ByteArray = byteArray,
                        DisplayOrder = loadOrder
                    });
                }
            }
            
            var exhibit = await _exhibitService.CreateExhibit(dto, employeeId);
            return CreatedAtAction(nameof(GetById), new { id = exhibit.ExhibitId }, exhibit);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
    
    /// <summary>
    /// Получение изображения экспоната
    /// </summary>
    /// <param name="id">Идентификатор изображения</param>
    /// <returns>Файл изображения</returns>
    [HttpGet("images/{id}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(int id)
    {
        var image = await _context.ExhibitImages.FindAsync(id);
        if (image == null)
            return NotFound();
        
        var filePath = Path.Combine(_environment.WebRootPath, image.ImagePath.TrimStart('/'));
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        
        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var contentType = GetContentType(Path.GetExtension(filePath));
        
        return File(bytes, contentType);
    }
    
    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}