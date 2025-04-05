using Microsoft.EntityFrameworkCore;

namespace MuseumService.Models.Services;

// ScoreService.cs
public class ScoreService
{
    private readonly AppDbContext _context;
    
    public ScoreService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ScoreDto>> GetAllScores()
    {
        return await _context.Scores
            .Include(s => s.Game)
            .OrderByDescending(s => s.ScoreValue)
            .Select(s => new ScoreDto
            {
                ScoreId = s.ScoreId,
                GameId = s.GameId,
                GameName = s.Game.GameName,
                PlayerName = s.PlayerName,
                ScoreValue = s.ScoreValue,
                PlayedAt = s.PlayedAt
            })
            .ToListAsync();
    }
    
    public async Task<List<ScoreDto>> GetScoresByGame(int gameId)
    {
        return await _context.Scores
            .Where(s => s.GameId == gameId)
            .Include(s => s.Game)
            .OrderByDescending(s => s.ScoreValue)
            .Select(s => new ScoreDto
            {
                ScoreId = s.ScoreId,
                GameId = s.GameId,
                GameName = s.Game.GameName,
                PlayerName = s.PlayerName,
                ScoreValue = s.ScoreValue,
                PlayedAt = s.PlayedAt
            })
            .ToListAsync();
    }
    
    public async Task<ScoreDto> AddScore(CreateScoreDto dto)
    {
        var score = new Score
        {
            GameId = dto.GameId,
            PlayerName = dto.PlayerName,
            ScoreValue = dto.ScoreValue,
            PlayedAt = DateTime.UtcNow
        };
        
        _context.Scores.Add(score);
        await _context.SaveChangesAsync();
        
        return await _context.Scores
            .Where(s => s.ScoreId == score.ScoreId)
            .Include(s => s.Game)
            .Select(s => new ScoreDto
            {
                ScoreId = s.ScoreId,
                GameId = s.GameId,
                GameName = s.Game.GameName,
                PlayerName = s.PlayerName,
                ScoreValue = s.ScoreValue,
                PlayedAt = s.PlayedAt
            })
            .FirstAsync();
    }
}