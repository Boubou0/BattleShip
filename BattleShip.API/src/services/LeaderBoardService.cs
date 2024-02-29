using System.Collections.Generic;
using System.Linq;
using BattleShip.Models;

public class LeaderboardService
{
    private readonly ApplicationDbContext _dbContext;

    public LeaderboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddOrUpdateUser(string username, int score)
    {
        var existingUser = _dbContext.LeaderboardItems.FirstOrDefault(u => u.PlayerName == username);
        
        if (existingUser != null)
        {
            existingUser.Score += score;
        }
        else
        {
            _dbContext.LeaderboardItems.Add(new LeaderboardItem { PlayerName = username, Score = score });
        }
        
        _dbContext.SaveChanges();
    }

    public void UpdateUserScore(string username, int newScore)
    {
        var user = _dbContext.LeaderboardItems.FirstOrDefault(u => u.PlayerName == username);
        if (user != null)
        {
            user.Score = newScore;
            _dbContext.SaveChanges();
        }
    }

    public List<LeaderboardItem> GetTop10Users()
    {
        return _dbContext.LeaderboardItems.OrderByDescending(u => u.Score).Take(10).ToList();
    }

    public void ResetLeaderboard()
    {
        _dbContext.LeaderboardItems.RemoveRange(_dbContext.LeaderboardItems);
        _dbContext.SaveChanges();
    }
}
