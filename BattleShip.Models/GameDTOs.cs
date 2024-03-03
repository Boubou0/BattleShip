namespace BattleShip.Models;
public record CreateGameDTO(Guid GameId, Dictionary<string, List<string>> Player1Position, int difficulte) {}
public record AttackDTO(string GameStatus, string AttackState, string MoveLabel, string sunkunBoat){}
public record listAttackDTO(AttackDTO playerMove, List<AttackDTO> listAiAttack, string winner){}
