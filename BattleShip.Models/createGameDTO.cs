namespace BattleShip.Models;
public record CreateGameDTO(Guid GameId, Dictionary<string, List<string>> Player1Position) {}

