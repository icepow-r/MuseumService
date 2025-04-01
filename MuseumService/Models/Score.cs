namespace MuseumService.Models;


// Score.cs
public class Score
{
    public int ScoreId { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; }
    public string PlayerName { get; set; }
    public int ScoreValue { get; set; }
    public DateTime PlayedAt { get; set; }
}