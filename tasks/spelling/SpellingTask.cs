using Godot;

[GlobalClass]
public partial class SpellingTask : Task
{
    [Export]
    private Button _checkButton;
    [Export]
    private Image _image;
    [Export]
    private LineEdit _characterEdit;
    [Export]
    private Label _leftLabel;
    [Export]
    private Label _rightLabel;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
