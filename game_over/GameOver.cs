using Godot;

public partial class GameOver : Control
{
	[Export]
	private Label _scoreLabel;

	public static int finalScore;

	public override void _Ready()
	{
		_scoreLabel.Text = "FINAL SCORE: " + finalScore;
	}

	public override void _Process(double delta)
	{
	}

	public void Retry()
	{
		GetTree().ChangeSceneToFile("res://task_framework/gameplay_ui.tscn");
	}

	public void Quit()
	{
		GetTree().ChangeSceneToFile("res://title_screen/title_screen.tscn");
	}
}
