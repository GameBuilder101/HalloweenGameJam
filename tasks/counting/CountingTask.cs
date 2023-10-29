using Godot;
using System;

[GlobalClass]
public partial class CountingTask : Task
{
    [Export]
    private Label _label;
    [Export]
    private LineEdit _numberInput;

    [Export]
    private Control _ballRegion;
    [Export]
    private Control _ball;
    private Control[] _balls;
    private int _correctColorBallCount;

    [Export]
    private int _minBallCount;
    [Export]
    private int _maxBallCount;
    /// <summary>
    /// Changes how difficulty influences the amount of balls.
    /// </summary>
    [Export]
    private double _difficultyMultiplier;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void SetDifficulty(int index)
    {
        base.SetDifficulty(index);

        _balls = new Control[(int)((1.0 + index * _difficultyMultiplier) * RandomNum.Next(_minBallCount, _maxBallCount + 1))];
        // Get a random correct ball color
        int correctColor = RandomNum.Next(3);

        for (int i = 0; i < _balls.Length; i++)
        {
            _balls[i] = (Control)_ball.Duplicate();
            _balls[i].AddThemeStyleboxOverride("panel", (StyleBoxFlat)_balls[i].GetThemeStylebox("panel").Duplicate());
            _balls[i].Visible = true;
            _ballRegion.AddChild(_balls[i]);

            // Assign a random color
            int randColor = RandomNum.Next(3);
            if (correctColor == randColor)
                _correctColorBallCount++;
            switch (randColor)
            {
                case 0:
                    ((StyleBoxFlat)_balls[i].GetThemeStylebox("panel")).BgColor = Color.Color8(255, 0, 0);
                    break;
                case 1:
                    ((StyleBoxFlat)_balls[i].GetThemeStylebox("panel")).BgColor = Color.Color8(0, 255, 0);
                    break;
                case 2:
                    ((StyleBoxFlat)_balls[i].GetThemeStylebox("panel")).BgColor = Color.Color8(0, 0, 255);
                    break;
            }

            bool ballNearby = false;
            do
            {
                _balls[i].Position = new Vector2((float)RandomNum.NextDouble() * _ballRegion.Size.X, (float)RandomNum.NextDouble() * _ballRegion.Size.Y);
                ballNearby = false;
                foreach (Control ball in _balls) // Make sure balls don't appear too close to each other
                {
                    if (ball != null && ball != _balls[i] && ball.Position.DistanceTo(_balls[i].Position) <= 15.0)
                        ballNearby = true;
                }
            }
            while (ballNearby);
        }

        switch (correctColor)
        {
            case 0:
                _label.Text = "Count the RED balls";
                break;
            case 1:
                _label.Text = "Count the GREEN balls";
                break;
            case 2:
                _label.Text = "Count the BLUE balls";
                break;
        }
    }

    public void Check()
    {
        if (_numberInput.Text.Trim() == _correctColorBallCount + "")
            Pass();
        else
            Fail();
    }
}
