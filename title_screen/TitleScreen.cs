using Godot;
using System;

public partial class TitleScreen : Control
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void Play()
	{
		GetTree().ChangeSceneToFile("res://task_framework/gameplay_ui.tscn");
	}

	public void Quit()
	{
		GetTree().Quit();
	}
}
