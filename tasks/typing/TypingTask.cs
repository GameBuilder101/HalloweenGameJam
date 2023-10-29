using Godot;
using System;

public partial class TypingTask : Task {

	[Export]
	private TextEdit textEditor;
	[Export]
	private Label givenText;
	
	public override void SetDifficulty(int index) {
		base.SetDifficulty(index);

		string[] wordBank = ((TypingDifficulty) Difficulties[index]
				).WordBank.Split("\\");
		givenText.Text = wordBank[RandomNum.Next(wordBank.Length)];
	}

	public void Submit() {
		string given = givenText.Text.Replace(" ", "").Replace(
			"\n", "").Replace("\t", "");
		string input = textEditor.Text.Replace(" ", "").Replace(
			"\n", "").Replace("\t", "");

		if (string.Equals(given, input,
				StringComparison.CurrentCultureIgnoreCase)) {
			Pass();
		} else {
			Fail();
		}
	}
}
