using Godot;
using System;

/// <summary>
/// Both holds data about a task and acts as the task "window".
/// </summary>
[GlobalClass]
public partial class Task : Control
{
	[Export]
	public string TaskName { get; private set; }
	[Export]
	public Texture2D TaskIcon { get; private set; }

	[Export]
	public Godot.Collections.Array<Resource> Difficulties { get; private set; }
	protected int _currentDifficultyIndex;
	public TaskDifficulty CurrentDifficulty { get { return (TaskDifficulty)Difficulties[_currentDifficultyIndex]; } }

	public double TimeLimit { get; private set; }
	public double CurrentTime { get; private set; }
	public bool IsOverdue { get { return CurrentTime <= 0.0; }  }

	[Export]
	private Button _dragButton;
	public bool IsDragging { get; private set; }
	private Vector2 _dragStartPos;
	private Vector2 _dragStartMousePos;

	[Export]
	private Label _titleLabel;
	[Export]
	private ProgressBar _timerBar;
	private StyleBoxFlat _timerBarStyleBox;
	[Export]
	private Gradient _timerGradient;

	protected Random RandomNum { get; private set; } = new Random();

	public override void _Ready()
	{
		_timerBarStyleBox = new StyleBoxFlat();
		_timerBar.AddThemeStyleboxOverride("fill", _timerBarStyleBox);
	}

	public override void _Process(double delta)
	{
		CurrentTime -= delta;
		if (IsOverdue)
			Overdue();
		if (!Visible)
			return;

		if (IsDragging)
		{
			Vector2 mousePos = GetViewport().GetMousePosition();
			Vector2 maxPos = ((Control)GetParent()).Size - Size;

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

		_timerBar.Value = CurrentTime / TimeLimit * 100.0;
		_timerBarStyleBox.BgColor = _timerGradient.Sample((float)(CurrentTime / TimeLimit));
	}

	public virtual void SetDifficulty(int index)
	{
		if (index < 0 || index >= Difficulties.Count)
			return;
		_currentDifficultyIndex = index;

		TimeLimit = CurrentDifficulty.MinTimeLimit + RandomNum.NextDouble() * CurrentDifficulty.MaxTimeLimit;
		CurrentTime = TimeLimit;
		_titleLabel.Text = TaskName + " [" + CurrentDifficulty.Score + " POINTS]";
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
		TaskManager.Instance.RemoveTask(this, TaskPassedState.Pass);
	}

	public virtual void Fail()
	{
		TaskManager.Instance.currentScore -= CurrentDifficulty.Score + CurrentDifficulty.Score / 2;
		TaskManager.Instance.RemoveTask(this, TaskPassedState.Fail);
	}

	protected virtual void Overdue()
	{
		TaskManager.Instance.currentScore -= CurrentDifficulty.Score / 2;
		TaskManager.Instance.RemoveTask(this, TaskPassedState.Overdue);
	}

	public void StartDragging()
	{
		IsDragging = true;
		_dragStartPos = Position;
		_dragStartMousePos = GetViewport().GetMousePosition();
		// Move the task to the top
		GetParent().MoveChild(this, GetParent().GetChildCount() - 1);
	}

	public void StopDragging()
	{
		IsDragging = false;
	}
}
