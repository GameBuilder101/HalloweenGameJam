using Godot;
using System;

public partial class BossManager : Node
{
    public static BossManager Instance { get; private set; }

    public double BossArrivalTimer { get; private set; }

    [Export]
    private double _initialMinBossArrivalTime;
    [Export]
    private double _initialMaxBossArrivalTime;
    [Export]
    private double _laterMinBossArrivalTime;
    [Export]
    private double _laterMaxBossArrivalTime;

    public int CurrentRequiredScore { get; private set; }
    [Export]
    private int _initialRequiredScore = 50;
    [Export]
    public int minAdditionalRequiredScore;
    [Export]
    public int maxAdditionalRequiredScore;

    [Export]
    private Label _requiredScoreLabel;

    Random random = new Random();

    public override void _Ready()
	{
        Instance = this;
        UpdateBossArrivalTimer();
    }

	public override void _Process(double delta)
	{
        BossArrivalTimer -= delta;
        if (BossArrivalTimer <= 0.0)
            ArriveBoss();
    }

    public void ArriveBoss()
    {
        UpdateBossArrivalTimer();
    }

    private void UpdateBossArrivalTimer()
    {
        // Update arrival timer
        BossArrivalTimer = Lerp(_initialMinBossArrivalTime, _laterMinBossArrivalTime, TaskManager.Instance.LaterPercent) +
            random.NextDouble() * Lerp(_laterMinBossArrivalTime, _laterMaxBossArrivalTime, TaskManager.Instance.LaterPercent);
        // Update required score
        CurrentRequiredScore += random.Next(minAdditionalRequiredScore, maxAdditionalRequiredScore);
        _requiredScoreLabel.Text = "NEXT GOAL: " + CurrentRequiredScore;
    }

    private double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }
}
