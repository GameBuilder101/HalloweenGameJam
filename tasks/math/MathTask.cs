using Godot;

[GlobalClass]
public partial class MathTask : Task
{
	public override void _Ready()
	{
		base._Ready();
		GD.Print(_currentDifficultyIndex); // Use the _currentDifficultyIndex to get the difficulty. 0 is the lowest and 4 is the highest.
		RandomNum.Next(5); // Use this for random numbers
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (true)
		{
			Pass();
		}
		else
		{
			Fail();
		}
	}
}
