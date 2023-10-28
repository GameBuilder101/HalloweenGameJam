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

    [Export]
    private Sprite2D _bossSprite;
    [Export]
    private float _bossPeekOffset = -100.0f;
    [Export]
    private double _bossPeekAnimationDuration;
    [Export]
    private double _bossPeekDuration;
    public double CurrentBossPeekTime { get; private set; } = -1.0; // Negative when not peeking
    public bool BossPeeking { get { return CurrentBossPeekTime >= 0.0; } }
    private bool _performedBossCheck;

    Random random = new Random();

    public override void _Ready()
	{
        Instance = this;
        UpdateBossArrivalTimer();
    }

	public override void _Process(double delta)
	{
        base._Process(delta);
        BossArrivalTimer -= delta;
        if (BossArrivalTimer <= 0.0)
            ArriveBoss();

        if (BossPeeking)
        {
            if (CurrentBossPeekTime <= _bossPeekAnimationDuration) // Move boss up
                _bossSprite.Offset = new Vector2(0.0f, (float)Lerp(0.0, _bossPeekOffset, CurrentBossPeekTime));
            else if (CurrentBossPeekTime >= _bossPeekDuration - _bossPeekAnimationDuration) // Move boss down
            {
                _bossSprite.Offset = new Vector2(0.0f, (float)Lerp(0.0, _bossPeekOffset, _bossPeekDuration - CurrentBossPeekTime));
                BossCheck();
            }

            CurrentBossPeekTime += delta;
            if (CurrentBossPeekTime >= _bossPeekDuration)
                StopBossPeekAnimation();
        }
    }

    public void ArriveBoss()
    {
        UpdateBossArrivalTimer();
        StartBossPeekAnimation();
    }

    private void UpdateBossArrivalTimer()
    {
        // Update arrival timer
        BossArrivalTimer = Lerp(_initialMinBossArrivalTime, _laterMinBossArrivalTime, TaskManager.Instance.LaterPercent) +
            random.NextDouble() * Lerp(_laterMinBossArrivalTime, _laterMaxBossArrivalTime, TaskManager.Instance.LaterPercent);
    }

    public void StartBossPeekAnimation()
    {
        _performedBossCheck = false;
        CurrentBossPeekTime = 0.0;
    }

    public void StopBossPeekAnimation()
    {
        CurrentBossPeekTime = -1.0;
    }

    public void BossCheck()
    {
        if (_performedBossCheck)
            return;
        _performedBossCheck = true;

        // Check if the player failed
        if (TaskManager.Instance.currentScore < CurrentRequiredScore)
        {
            GD.Print("Game over lol get gud");
            return;
        }

        // Update required score
        CurrentRequiredScore += random.Next(minAdditionalRequiredScore, maxAdditionalRequiredScore);
        _requiredScoreLabel.Text = "NEXT GOAL: " + CurrentRequiredScore;
    }

    private double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }
}
