using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class SpellingTask : Task
{
    [Export]
    private Button _checkButton;
    [Export]
    private TextureRect _image;
    [Export]
    private LineEdit _characterEdit;
    [Export]
    private Label _leftLabel;
    [Export]
    private Label _rightLabel;

    [Export]
    private Godot.Collections.Array<Resource> _diff0Words;
    [Export]
    private Godot.Collections.Array<Resource> _diff1Words;
    [Export]
    private Godot.Collections.Array<Resource> _diff2Words;

    private List<SpellingTaskWord> _currentWords = new List<SpellingTaskWord>();
    private SpellingTaskWord CurrentWord { get { return _currentWords[_completed]; } }
    private int _completed;
    private int _targetCompleted;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void SetDifficulty(int index)
    {
        base.SetDifficulty(index);
        _targetCompleted = 2;
        if (index > 0)
            _targetCompleted++;

        // Randomly select words
        while (_currentWords.Count < _targetCompleted)
        {
            SpellingTaskWord word = null;
            switch (index)
            {
                case 0:
                    word = (SpellingTaskWord)_diff0Words[RandomNum.Next(_diff0Words.Count)];
                    break;
                case 1:
                    word = (SpellingTaskWord)_diff1Words[RandomNum.Next(_diff1Words.Count)];
                    break;
                case 2:
                    word = (SpellingTaskWord)_diff2Words[RandomNum.Next(_diff2Words.Count)];
                    break;
            }
            if (!_currentWords.Contains(word))
                _currentWords.Add(word);
        }

        DisplayWord();
    }

    public void DisplayWord()
    {
        _checkButton.Text = "Check (" + _completed + "/" + _targetCompleted + ")";
        _image.Texture = CurrentWord.image;
        _leftLabel.Text = CurrentWord.left;
        _rightLabel.Text = CurrentWord.right;
        _characterEdit.MaxLength = CurrentWord.target.Length;
        _characterEdit.Text = "";
    }

    public void Check()
    {
        if (_characterEdit.Text.Trim().ToLower() != CurrentWord.target.ToLower())
        {
            Fail();
            return;
        }
        else if (_completed + 1 >= _targetCompleted)
        {
            Pass();
            return;
        }

        _completed++;
        DisplayWord();
    }
}
