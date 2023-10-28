using Godot;

[GlobalClass]
public partial class MathTask : Task
{
	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
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
