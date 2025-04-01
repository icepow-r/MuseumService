namespace MuseumService.Models;

// Models/ExhibitImage.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ExhibitImage
{
    [Key] // Явное указание первичного ключа
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
    public int ImageId { get; set; }
    
    [ForeignKey("Exhibit")] // Явное указание внешнего ключа
    public int ExhibitId { get; set; }
    
    public string ImagePath { get; set; }
    public string AltText { get; set; }
    public int DisplayOrder { get; set; }
    
    public Exhibit Exhibit { get; set; } // Навигационное свойство
}