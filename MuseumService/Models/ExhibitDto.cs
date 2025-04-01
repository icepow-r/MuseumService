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

public class ExhibitImageDto
{
    public string ImagePath { get; set; }
    public string AltText { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateExhibitDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<ExhibitImageDto> Images { get; set; } = new();
}

public class UpdateExhibitDto
{
    public string Title { get; set; }
    public string Description { get; set; }
}