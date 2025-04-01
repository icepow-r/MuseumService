namespace MuseumService.Models;

// ScoreDto.cs
public class ScoreDto
{
    public int ScoreId { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; }
    public string PlayerName { get; set; }
    public int ScoreValue { get; set; }
    public DateTime PlayedAt { get; set; }
}

public class CreateScoreDto
{
    public int GameId { get; set; }
    public string PlayerName { get; set; }
    public int ScoreValue { get; set; }
}