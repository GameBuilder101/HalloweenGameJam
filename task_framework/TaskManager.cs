using Godot;
using System;
using System.Collections.Generic;

public partial class TaskManager : Node
{
    public static TaskManager Instance { get; private set; }

    /// <summary>
    /// The player's current score.
    /// </summary>
    public double currentScore;
    [Export]
    private Label _scoreLabel;

    [Export]
    public Godot.Collections.Array<PackedScene> Tasks { get; private set; }
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
            ((StyleBoxFlat)((Control)activeTaskListItem).GetThemeStylebox("fill", "")).BgColor = _timerGradient.Sample((float)progress);
            i++;
        }

        _scoreLabel.Text = "Score: " + currentScore;
    }

    public void AssignTask()
    {
        // Add a new task
        Task task = Tasks[random.Next(Tasks.Count)].Instantiate() as Task;
        _desktop.AddChild(task);
        task.Close();
        task.SetDifficulty(0);
        _activeTasks.Add(task);

        // Add the task list item
        Node taskListItem = _taskListItem.Duplicate();
        _taskList.AddChild(taskListItem);
        ((Control)taskListItem).Visible = true;
        taskListItem.GetNode<Button>("Button").Pressed += delegate { task.Open(); };
        taskListItem.GetNode<Label>("Button/Name").Text = task.TaskName;
        taskListItem.GetNode<Label>("Button/Points").Text = "[" + task.CurrentDifficulty.Score + " POINTS]";
        taskListItem.GetNode<TextureRect>("Button/Icon").Texture = task.TaskIcon;
        _activeTaskListItems.Add(taskListItem);

        // Reset timer
        TaskArrivalTimer = Lerp(_initialMinTaskArrivalTime, _laterMinTaskArrivalTime, LaterPercent) +
            Lerp(_initialMaxTaskArrivalTime, _laterMaxTaskArrivalTime, LaterPercent) * random.NextDouble();
    }

    public void RemoveTask(Task task)
    {
        int index = _activeTasks.IndexOf(task);
        _activeTasks[index].QueueFree();
        _activeTaskListItems[index].QueueFree();
        _activeTasks.RemoveAt(index);
        _activeTaskListItems.RemoveAt(index);

        if (_activeTasks.Count <= 0)
            TaskArrivalTimer = 1.0; // Add a small delay instead of immediately assigning a new task
    }

    private double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }
}
