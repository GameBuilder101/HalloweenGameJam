using Godot;
using System;


public partial class TitleScreen : Control
{
	
[Export]
	private AudioStreamPlayer _FirstTheme;
	public override void _Ready()
	{
		_FirstTheme.Play();
		
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
	
	private void _on_first_day_finished()
{
	_FirstTheme.Play();
	// Replace with function body.
}
}



