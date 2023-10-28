using Godot;

[GlobalClass]
public partial class TaskDifficulty : Resource
{
    [Export]
    public double MinTimeLimit { get; private set; }
    [Export]
    public double MaxTimeLimit { get; private set; }

    [Export]
    public int Score { get; private set; }
    [Export]
    public int FailPenalty { get; private set; }
    [Export]
    public int OverduePenalty { get; private set; }
}
