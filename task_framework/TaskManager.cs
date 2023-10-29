using Godot;
using System;
using System.Collections.Generic;


public partial class TaskManager : Node
{
	public static TaskManager Instance { get; private set; }

	/// <summary>
	/// The player's current score.
	/// </summary>
	public int currentScore;
	[Export]
	private Label _scoreLabel;

	[Export]
	public Godot.Collections.Array<PackedScene> Tasks { get; private set; }
	[Export]
	private PackedScene Secret;
	private List<Task> _activeTasks;
	[Export]
	private Node _desktop;

	public double TotalTime { get; private set; }
	public double TaskArrivalTimer { get; private set; }
	[Export]
	public int maxTasks = 10; // Add a cap so there's not a crazy stupid amount of tasks at once
	/// <summary>
	/// "Later" is just how long it takes till the cap on task arrival time is reached.
	/// </summary>
	[Export]
	private double _laterThreshold;
	public double LaterPercent
	{
		get
		{
			if (TotalTime > _laterThreshold)
				return 1.0;
			return TotalTime / _laterThreshold;
		}
	}

	[Export]
	private double _initialMinTaskArrivalTime;
	[Export]
	private double _initialMaxTaskArrivalTime;
	[Export]
	private double _laterMinTaskArrivalTime;
	[Export]
	private double _laterMaxTaskArrivalTime;

	[Export]
	public Godot.Collections.Array<double> InitialDifficultyChances { get; private set; }
	[Export]
	public Godot.Collections.Array<double> LaterDifficultyChances { get; private set; }

	[Export]
	private Node _taskList;
	private List<Node> _activeTaskListItems;
	[Export]
	private Node _taskListItem;
	[Export]
	private Gradient _timerGradient;

	[Export]
	private Sprite2D _taskCorrectIndicator;
	[Export]
	private Texture2D _taskPassIcon;
	[Export]
	private Texture2D _taskFailIcon;
	[Export]
	private double _correctIndicatorAnimDuration;
	private double _correctIndicatorAnimTime = -1.0;

	[Export]
	private AudioStreamPlayer _newTaskAudio;
	[Export]
	private AudioStreamPlayer _failedTaskAudio;
	[Export]
	private AudioStreamPlayer _correctTaskAudio;
	[Export]
	private AudioStreamPlayer _warningAudio;

    [Export]
    private AudioStreamPlayer _normalMusic;
    [Export]
    private AudioStreamPlayer _intenseMusic;
	[Export]
	private int _intenseMusicThreshold = 5;
	private double _transitionIntenseMusicTime = -1.0;

    Random random = new Random();

	public override void _Ready()
	{
		Instance = this;
		_activeTasks = new List<Task>();
		_activeTaskListItems = new List<Node>();

		TaskArrivalTimer = 3.0; // Give an initial first task delay
	}

	public override void _Process(double delta)
	{
		TotalTime += delta;
		TaskArrivalTimer -= delta;
		if (TaskArrivalTimer <= 0.0 && _activeTasks.Count < maxTasks)
			AssignTask();

		int i = 0;
		foreach (Node activeTaskListItem in _activeTaskListItems)
		{
			double progress = _activeTasks[i].CurrentTime / _activeTasks[i].TimeLimit;
			activeTaskListItem.GetNode<ProgressBar>("Button/Timer").Value = progress * 100.0;
			activeTaskListItem.GetNode<Label>("Button/TimerLabel").Text =
				Math.Floor(_activeTasks[i].CurrentTime / 60.0) + ":" + (_activeTasks[i].CurrentTime % 60.0).ToString("00");
			((StyleBoxFlat)activeTaskListItem.GetNode<ProgressBar>("Button/Timer").GetThemeStylebox("fill")).BgColor = _timerGradient.Sample((float)progress);
			i++;
		}

		_scoreLabel.Text = "SCORE: " + currentScore;

		// Correct indicator animaions
		if (_correctIndicatorAnimTime >= 0.0)
		{
			float progress = (float)(_correctIndicatorAnimTime / _correctIndicatorAnimDuration);
			// Fade out the indicator
			_taskCorrectIndicator.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f - progress);
			// Move the indicator up
			_taskCorrectIndicator.Offset = new Vector2(0.0f, progress * -100.0f);
			_correctIndicatorAnimTime += delta;
			if (_correctIndicatorAnimTime >= _correctIndicatorAnimDuration)
				StopCorrectIndicatorAnim();
		}

		if (_activeTasks.Count >= _intenseMusicThreshold && _transitionIntenseMusicTime < 0.0 && !_intenseMusic.Playing)
            _transitionIntenseMusicTime = 0.0;
		if (_transitionIntenseMusicTime >= 0.0)
		{
			_transitionIntenseMusicTime += delta;
			_normalMusic.VolumeDb = (float)Lerp(0.0, -80.0, _transitionIntenseMusicTime);
			if (_transitionIntenseMusicTime >= 1.0)
			{
				_transitionIntenseMusicTime = -1.0;
				_normalMusic.Stop();
				_intenseMusic.Play();
			}
		}
		if (!_intenseMusic.Playing && !_normalMusic.Playing)
		{
			_normalMusic.VolumeDb = 0.0f;
            _normalMusic.Play();
        }
    }

	public void AssignTask()
	{
		// Add a new task
		Task task;
		if (random.Next(1000) == 0) {
			task = (Task) Secret.Instantiate();
		} else {
			task = (Task)Tasks[random.Next(Tasks.Count)].Instantiate();
		}
		_desktop.AddChild(task);
		// Plays the sound effect
		_newTaskAudio.Play();
		// Move the task to a random position
		task.Position = new Vector2((float)random.NextDouble() * (((Control)_desktop).Size.X - task.Size.X),
			(float)random.NextDouble() * (((Control)_desktop).Size.Y - task.Size.Y));
		task.Close();
		_activeTasks.Add(task);

		// DIFFICULTY MATH ---------------------
		// Calculate total weight
		double totalWeight = 0.0;
		for (int i = 0; i < task.Difficulties.Count; i++)
			totalWeight += Lerp(InitialDifficultyChances[i], LaterDifficultyChances[i], LaterPercent);
		// Get random weight
		double randomWeight = random.NextDouble() * totalWeight;
		totalWeight = 0.0;
		int difficultyIndex = 0;
		// Find which difficulty that random weight falls under
		for (int i = 0; i < task.Difficulties.Count; i++)
		{
			totalWeight += Lerp(InitialDifficultyChances[i], LaterDifficultyChances[i], LaterPercent);
			if (randomWeight < totalWeight)
			{
				difficultyIndex = i;
				break;
			}
		}
		// Set the difficulty
		task.SetDifficulty(difficultyIndex);

		// Add the task list item
		Node taskListItem = _taskListItem.Duplicate();
		_taskList.AddChild(taskListItem);
		((Control)taskListItem).Visible = true;
		taskListItem.GetNode<Button>("Button").Pressed += delegate { task.Open(); };
		taskListItem.GetNode<Label>("Button/Name").Text = task.TaskName;
		taskListItem.GetNode<Label>("Button/Points").Text = "[" + task.CurrentDifficulty.Score + " POINTS]";
		taskListItem.GetNode<TextureRect>("Button/Icon").Texture = task.TaskIcon;
		taskListItem.GetNode<ProgressBar>("Button/Timer").AddThemeStyleboxOverride("fill", new StyleBoxFlat());
		_activeTaskListItems.Add(taskListItem);

		// Reset timer
		TaskArrivalTimer = Lerp(_initialMinTaskArrivalTime, _laterMinTaskArrivalTime, LaterPercent) +
			Lerp(_initialMaxTaskArrivalTime, _laterMaxTaskArrivalTime, LaterPercent) * random.NextDouble();
	}

	public void RemoveTask(Task task, TaskPassedState state = TaskPassedState.Overdue)
	{
		int index = _activeTasks.IndexOf(task);
		_activeTasks[index].QueueFree();
		_activeTaskListItems[index].QueueFree();
		_activeTasks.RemoveAt(index);
		_activeTaskListItems.RemoveAt(index);

		if (_activeTasks.Count <= 0)
			TaskArrivalTimer = 1.0; // Add a small delay instead of immediately assigning a new task

		if (state == TaskPassedState.Pass)
		{
			_taskCorrectIndicator.Texture = _taskPassIcon;
			// Plays the sound effect
			_correctTaskAudio.Play();
			StartCorrectIndicatorAnim();
		}
		else if (state == TaskPassedState.Fail)
		{
			_taskCorrectIndicator.Texture = _taskFailIcon;
			// Plays the sound effect
			_failedTaskAudio.Play();
			StartCorrectIndicatorAnim();
		}
	}

	private void StartCorrectIndicatorAnim()
	{
		_correctIndicatorAnimTime = 0.0;
		
		// Move to the cursor
		_taskCorrectIndicator.Position = _taskCorrectIndicator.GetViewport().GetMousePosition();
		_taskCorrectIndicator.Visible = true;
	}

	private void StopCorrectIndicatorAnim()
	{
		_correctIndicatorAnimTime = -1.0;
		_taskCorrectIndicator.Visible = false;
	}

	private double Lerp(double a, double b, double t)
	{
		return a + (b - a) * t;
	}
}

public enum TaskPassedState { Pass, Fail, Overdue }
