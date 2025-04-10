using Microsoft.EntityFrameworkCore;

namespace MuseumService.Models.Services;

// ExhibitService.cs
public class ExhibitService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    
    public ExhibitService(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    
    public async Task<List<ExhibitDto>> GetAllExhibits()
    {
        var exhibits = await _context.Exhibits
            .Include(e => e.Images)
            .OrderBy(e => e.Title)
            .ToListAsync();

        var result = new List<ExhibitDto>();

        foreach (var exhibit in exhibits)
        {
            var exhibitDto = new ExhibitDto
            {
                ExhibitId = exhibit.ExhibitId,
                Title = exhibit.Title,
                Description = exhibit.Description,
                AddedAt = exhibit.AddedAt,
                UpdatedAt = exhibit.UpdatedAt,
                Images = new List<ExhibitImageDto>()
            };
            
            foreach (var image in exhibit.Images)
            {
                var imagePath = image.ImagePath.TrimStart('/');
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
                byte[] imageBytes = Array.Empty<byte>();
                
                if (File.Exists(fullPath))
                {
                    imageBytes = await File.ReadAllBytesAsync(fullPath);
                }

                exhibitDto.Images.Add(new ExhibitImageDto
                {
                    ImagePath = image.ImagePath,
                    AltText = image.AltText,
                    DisplayOrder = image.DisplayOrder,
                    ImageExt = Path.GetExtension(image.ImagePath).TrimStart('.'),
                    ByteArray = imageBytes
                });
            }

            result.Add(exhibitDto);
        }

        return result;
    }
    
    public async Task<ExhibitDto?> GetExhibitById(int id)
    {
        var exhibit = await _context.Exhibits
            .Where(e => e.ExhibitId == id)
            .Include(e => e.Images)
            .FirstOrDefaultAsync();

        if (exhibit == null)
            return null;

        var result = new ExhibitDto
        {
            ExhibitId = exhibit.ExhibitId,
            Title = exhibit.Title,
            Description = exhibit.Description,
            AddedAt = exhibit.AddedAt,
            UpdatedAt = exhibit.UpdatedAt,
            Images = new List<ExhibitImageDto>()
        };

        foreach (var image in exhibit.Images)
        {
            var imagePath = image.ImagePath.TrimStart('/');
            var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
            byte[] imageBytes = [];
            
            if (File.Exists(fullPath))
            {
                imageBytes = await File.ReadAllBytesAsync(fullPath);
            }

            result.Images.Add(new ExhibitImageDto
            {
                ImagePath = image.ImagePath,
                AltText = image.AltText,
                DisplayOrder = image.DisplayOrder,
                ImageExt = Path.GetExtension(image.ImagePath).TrimStart('.'),
                ByteArray = imageBytes
            });
        }

        return result;
    }
    
    public async Task<ExhibitDto> CreateExhibit(CreateExhibitDto dto, int employeeId)
    {
        var exhibit = new Exhibit
        {
            Title = dto.Title,
            Description = dto.Description,
            EmployeeId = employeeId,
            AddedAt = DateTime.UtcNow,
            Images = new List<ExhibitImage>()
        };
        
        _context.Exhibits.Add(exhibit);
        await _context.SaveChangesAsync();
        
        if (dto.Images != null && dto.Images.Count > 0)
        {
            var imageDirectory = Path.Combine(_environment.WebRootPath, "images");
            
             if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }
            
            foreach (var imageDto in dto.Images)
            {
                if (imageDto.ByteArray != null && imageDto.ByteArray.Length > 0)
                {
                     var fileName = $"{Guid.NewGuid()}.{imageDto.ImageExt}";
                    var filePath = Path.Combine(imageDirectory, fileName);
                    
                    await File.WriteAllBytesAsync(filePath, imageDto.ByteArray);
                    
                    var exhibitImage = new ExhibitImage
                    {
                        ExhibitId = exhibit.ExhibitId,
                        ImagePath = $"/images/{fileName}",
                        AltText = string.IsNullOrEmpty(imageDto.AltText) ? exhibit.Title : imageDto.AltText,
                        DisplayOrder = dto.LoadOrder
                    };
                    
                    _context.ExhibitImages.Add(exhibitImage);
                }
            }
            
            await _context.SaveChangesAsync();
        }
        
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