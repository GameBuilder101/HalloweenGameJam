using Godot;
using System;
using System.Diagnostics;

/// <summary>
/// Both holds data about a task and acts as the task "window".
/// </summary>
public partial class Task : Control
{
	[Export]
	public string TaskName { get; private set; }
    [Export]
    public Texture2D TaskIcon { get; private set; }

    [Export]
	public Godot.Collections.Array<Resource> Difficulties { get; private set; }
	private int _currentDifficultyIndex;
	public TaskDifficulty CurrentDifficulty { get { return Difficulties[_currentDifficultyIndex] as TaskDifficulty; } }

	public double TimeLimit { get; private set; }
	public double CurrentTime { get; private set; }
	public bool IsOverdue { get { return CurrentTime <= 0.0; }  }

	[Export]
	private Button _dragButton;
	public bool IsDragging { get; private set; }
	private Vector2 _dragStartPos;
	private Vector2 _dragStartMousePos;

	Random random = new Random();

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		CurrentTime -= delta;
		if (IsOverdue)
            Overdue();

		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 maxPos = ((Control)GetParent()).Size - Size;

        if (IsDragging)
		{
			Position = _dragStartPos + (mousePos - _dragStartMousePos);

			if (Position.X < 0.0f)
				Position = new Vector2(0.0f, Position.Y);
			else if (Position.X > maxPos.X)
				Position = new Vector2(maxPos.X, Position.Y);
            if (Position.Y < 0.0f)
                Position = new Vector2(Position.X, 0.0f);
            else if (Position.Y > maxPos.Y)
                Position = new Vector2(Position.X, maxPos.Y);
        }
    }

	public void SetDifficulty(int index)
	{
		if (index < 0 || index >= Difficulties.Count)
			return;
		_currentDifficultyIndex = index;
		TimeLimit = CurrentDifficulty.MinTimeLimit + random.NextDouble() * CurrentDifficulty.MaxTimeLimit;
		CurrentTime = TimeLimit;
	}

	/// <summary>
	/// Called when the task is clicked on in the list.
	/// </summary>
	public virtual void Open()
	{
		Visible = true;
	}

	/// <summary>
	/// Cakked when the close button is pressed.
	/// </summary>
	public virtual void Close()
	{
		Visible = false;
	}

	public virtual void Pass()
	{
		TaskManager.Instance.currentScore += CurrentDifficulty.Score;
		TaskManager.Instance.RemoveTask(this);
	}

	public virtual void Fail()
	{
        TaskManager.Instance.currentScore -= CurrentDifficulty.FailPenalty;
        TaskManager.Instance.RemoveTask(this);
    }

	protected virtual void Overdue()
	{
        TaskManager.Instance.currentScore -= CurrentDifficulty.OverduePenalty;
        TaskManager.Instance.RemoveTask(this);
    }

	public void StartDragging()
	{
		IsDragging = true;
		_dragStartPos = Position;
		_dragStartMousePos = GetViewport().GetMousePosition();
	}

	public void StopDragging()
	{
        IsDragging = false;
    }
}
