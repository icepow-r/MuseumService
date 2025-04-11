namespace MuseumService.Models;


// Exhibit.cs
public class Exhibit
{
    public int ExhibitId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Collection { get; set; }
    public string Era { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<ExhibitImage> Images { get; set; }
}