using Godot;
using System;

[GlobalClass]
public partial class SortingBinsTask : Task
{
    [Export]
    private Godot.Collections.Array<Control> _bins;

    [Export]
    private Control _paperSpawnRegion;
    [Export]
    private Control _paperRegion;
    [Export]
    private Control _paper;
    private Control[] _papers;
    private int _sortedPapers;

    [Export]
    private Color _redColor;
    [Export]
    private Color _yellowColor;
    [Export]
    private Color _greenColor;
    [Export]
    private Color _blueColor;

    [Export]
    private int _minPaperCount;
    [Export]
    private int _maxPaperCount;
    /// <summary>
    /// Changes how difficulty influences the amount of balls.
    /// </summary>
    [Export]
    private double _difficultyMultiplier;

    private Control _draggingPaper;
    private Vector2 _dragStartPos;
    private Vector2 _dragStartMousePos;

    private Random _random = new Random();

    public override void _Ready()
    {
        base._Ready();
        for (int i = 0; i < _bins.Count; i++)
        {
            string targetLabel = "" + (i + 1);
            _bins[i].MouseEntered += delegate { BinMouseEntered(targetLabel); };
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!Visible)
            return;

        if (_draggingPaper != null)
        {
            Vector2 mousePos = _draggingPaper.GetViewport().GetMousePosition();
            Vector2 maxPos = ((Control)_draggingPaper.GetParent()).Size - _draggingPaper.Size;

            _draggingPaper.Position = _dragStartPos + (mousePos - _dragStartMousePos);

            if (_draggingPaper.Position.X < 0.0f)
                _draggingPaper.Position = new Vector2(0.0f, _draggingPaper.Position.Y);
            else if (_draggingPaper.Position.X > maxPos.X)
                _draggingPaper.Position = new Vector2(maxPos.X, _draggingPaper.Position.Y);
            if (_draggingPaper.Position.Y < 0.0f)
                _draggingPaper.Position = new Vector2(_draggingPaper.Position.X, 0.0f);
            else if (_draggingPaper.Position.Y > maxPos.Y)
                _draggingPaper.Position = new Vector2(_draggingPaper.Position.X, maxPos.Y);
        }
    }

    public override void SetDifficulty(int index)
    {
        base.SetDifficulty(index);

        // Add more bin colors
        if (index >= 1)
            _bins[2].Visible = true;
        if (index >= 2)
            _bins[3].Visible = true;

        // Create the papers
        _papers = new Control[(int)((1.0 + index * _difficultyMultiplier) * _random.Next(_minPaperCount, _maxPaperCount + 1))];
        for (int i = 0; i < _papers.Length; i++)
        {
            _papers[i] = (Control)_paper.Duplicate();
            _papers[i].Visible = true;
            _papers[i].Position = new Vector2((float)RandomNum.NextDouble() * _paperSpawnRegion.Size.X,
                (float)RandomNum.NextDouble() * _paperSpawnRegion.Size.Y);
            _paperRegion.AddChild(_papers[i]);

            Button button = _papers[i].GetNode<Button>("Button");

            switch (RandomNum.Next(index + 2)) // Assign the paper a random color depending on how many bins are avaliable
            {
                case 0:
                    _papers[i].SelfModulate = _redColor;
                    button.Text = "1";
                    break;
                case 1:
                    _papers[i].SelfModulate = _yellowColor;
                    button.Text = "2";
                    break;
                case 2:
                    _papers[i].SelfModulate = _greenColor;
                    button.Text = "3";
                    break;
                case 3:
                    _papers[i].SelfModulate = _blueColor;
                    button.Text = "4";
                    break;
            }

            Control paper = _papers[i];
            button.ButtonDown += delegate { StartDragging(paper); };
            button.ButtonUp += delegate { StopDragging(paper); };
        }
    }

    public void StartDragging(Control paper)
    {
        if (_draggingPaper != null)
            return;
        _draggingPaper = paper;
        _dragStartPos = _draggingPaper.Position;
        _dragStartMousePos = _draggingPaper.GetViewport().GetMousePosition();
    }

    public void StopDragging(Control paper)
    {
        if (_draggingPaper != paper)
            return;
        _draggingPaper = null;
    }

    public void BinMouseEntered(string targetLabel)
    {
        if (_draggingPaper == null)
            return;

        if (_draggingPaper.GetNode<Button>("Button").Text != targetLabel) // If not the correct paper type
        {
            Fail();
            return;
        }

        _draggingPaper.Visible = false;
        _sortedPapers++;
        if (_sortedPapers >= _papers.Length)
            Pass();
    }
}
