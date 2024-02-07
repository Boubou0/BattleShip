namespace BattleShip.Models;
public record CreateGameDTO(Guid GameId, Dictionary<string, List<string>> Player1Position) {}
public record AttackDTO(string GameStatus, string AttackState, string MoveLabel){}
public record listAttackDTO(AttackDTO playerMove, List<AttackDTO> listAiAttack, string winner){}
