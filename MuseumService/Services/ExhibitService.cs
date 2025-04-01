using Microsoft.EntityFrameworkCore;

namespace MuseumService.Models.Services;

// ExhibitService.cs
public class ExhibitService
{
    private readonly AppDbContext _context;
    
    public ExhibitService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ExhibitDto>> GetAllExhibits()
    {
        return await _context.Exhibits
            .Include(e => e.Images)
            .OrderBy(e => e.Title)
            .Select(e => new ExhibitDto
            {
                ExhibitId = e.ExhibitId,
                Title = e.Title,
                Description = e.Description,
                AddedAt = e.AddedAt,
                UpdatedAt = e.UpdatedAt,
                Images = e.Images.Select(i => new ExhibitImageDto
                {
                    ImagePath = i.ImagePath,
                    AltText = i.AltText,
                    DisplayOrder = i.DisplayOrder
                }).ToList()
            })
            .ToListAsync();
    }
    
    public async Task<ExhibitDto?> GetExhibitById(int id)
    {
        return await _context.Exhibits
            .Where(e => e.ExhibitId == id)
            .Include(e => e.Images)
            .Select(e => new ExhibitDto
            {
                // аналогично GetAllExhibits
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<ExhibitDto> CreateExhibit(CreateExhibitDto dto, int employeeId)
    {
        var exhibit = new Exhibit
        {
            Title = dto.Title,
            Description = dto.Description,
            EmployeeId = employeeId,
            AddedAt = DateTime.UtcNow,
            Images = dto.Images.Select((img, index) => new ExhibitImage
            {
                ImagePath = img.ImagePath,
                AltText = img.AltText,
                DisplayOrder = img.DisplayOrder
            }).ToList()
        };
        
        _context.Exhibits.Add(exhibit);
        await _context.SaveChangesAsync();
        
        return await GetExhibitById(exhibit.ExhibitId);
    }
    
    public async Task<ExhibitDto?> UpdateExhibit(int id, UpdateExhibitDto dto)
    {
        var exhibit = await _context.Exhibits
            .Include(e => e.Images)
            .FirstOrDefaultAsync(e => e.ExhibitId == id);
            
        if (exhibit == null)
            return null;
            
        exhibit.Title = dto.Title;
        exhibit.Description = dto.Description;
        exhibit.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return await GetExhibitById(exhibit.ExhibitId);
    }
    
    public async Task<bool> DeleteExhibit(int id)
    {
        var exhibit = await _context.Exhibits.FindAsync(id);
        if (exhibit == null)
            return false;
            
        _context.Exhibits.Remove(exhibit);
        await _context.SaveChangesAsync();
        return true;
    }
}