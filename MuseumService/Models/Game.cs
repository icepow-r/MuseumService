namespace MuseumService.Models;

// Game.cs
public class Game
{
    public int GameId { get; set; }
    public string GameName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string GameType { get; set; } // "quiz" или "game"
    
    public ICollection<Score> Scores { get; set; }
}





