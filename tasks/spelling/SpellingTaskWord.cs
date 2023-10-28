using Godot;

[GlobalClass]
public partial class SpellingTaskWord : Resource
{
    [Export]
    public string left;
    [Export]
    public string target;
    [Export]
    public string right;

    [Export]
    public Texture2D image;
}
