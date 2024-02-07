public class AttackResponseDTO
{
    public AttackDTO attack { get; set; }
}

public class AttackDTO
{
    public string winner { get; set; }
    public string gameStatus { get; set; }
    public string attackState { get; set; }
    public string moveLabel { get; set; }
}