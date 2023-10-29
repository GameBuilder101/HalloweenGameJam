using Godot;

[GlobalClass]
public partial class MathTask : Task
{
	[Export]
	private LaTeX display;
	[Export]
	private TextEdit textInput;

	private int answer;

	public override void SetDifficulty(int index) {
		base.SetDifficulty(index);

		int randFactor = RandomNum.Next(9) + 1;
		string latexString = "{0}";
		answer = randFactor;

		switch (CurrentDifficulty.Score) {
			case 10: {
				switch (RandomNum.Next(4)) {
					case 0: {
						latexString = "\\frac[2520][{0}]";
						answer = 2520 / randFactor;
						break;
					}
					case 1: {
						latexString = "{0}^2";
						answer = randFactor * randFactor;
						break;
					}
					case 2: {
						latexString = "1^{0}";
						answer = 1;
						break;
					}
					case 3: {
						latexString = "\\sqrt[{0}^2]";
						answer = randFactor;
						break;
					}
				}
				break;
			}
			case 30: {
				switch (RandomNum.Next(4)) {
					case 0: {
						latexString = "\\frac{}{}";
						break;
					}
				}
				break;
			}
		}

		string formattedString = string.Format(latexString, randFactor).Replace(
				'[', '{').Replace(']', '}');
		display.LatexExpression = formattedString;
		display.Render();
	}
	
	private void Submit() {
		int result;
		bool success = int.TryParse(textInput.Text, out result);
		if (!success) return;
		if (result == answer) {
			Pass();
		} else {
			Fail();
		}
	}
}
