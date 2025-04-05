using System.ComponentModel.DataAnnotations;

namespace MuseumService.Models;

// ExhibitDto.cs
public class ExhibitDto
{
    public int ExhibitId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ExhibitImageDto> Images { get; set; } = new();
}

public class CreateExhibitDto
{
    [Required(ErrorMessage = "Название экспоната обязательно")]
    [StringLength(255, ErrorMessage = "Название не может быть длиннее 255 символов")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Описание экспоната обязательно")]
    public string Description { get; set; }
    
    public List<ExhibitImageDto> Images { get; set; } = new();
}

public class ExhibitImageDto
{
    [Required(ErrorMessage = "Путь к изображению обязателен")]
    [StringLength(255, ErrorMessage = "Путь не может быть длиннее 255 символов")]
    public string ImagePath { get; set; }
    
    [StringLength(255, ErrorMessage = "Альтернативный текст не может быть длиннее 255 символов")]
    public string AltText { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Порядок отображения должен быть неотрицательным числом")]
    public int DisplayOrder { get; set; }
}

public class UpdateExhibitDto
{
    [Required(ErrorMessage = "Название экспоната обязательно")]
    [StringLength(255, ErrorMessage = "Название не может быть длиннее 255 символов")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Описание экспоната обязательно")]
    public string Description { get; set; }
}