using Godot;
using System;
using System.Diagnostics;

/// <summary>
/// Both holds data about a task and acts as the task "window".
/// </summary>
public partial class DatesTask : Task
{
	private int filesLength;
	private File[] files;
	[Export]
	private ItemList filesList;

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (!Visible)
			return;
		
		for (int i = 0; i < filesLength - 1; i++) {
			if (!files[i].EarlierThan(files[i + 1])) return;
		}
		
		Pass();
	}
	
	public override void SetDifficulty(int index) {
		base.SetDifficulty(index);

		int max_files = CurrentDifficulty.Score / 5 + 1;
		if (max_files > 10) {
			max_files = 10;
		}
		files = new File[max_files];
		for (int i = 0; i < max_files; i++) {
			files[i] = new File() {
				year = RandomNum.Next(4000),
				month = RandomNum.Next(13) + 1,
				day = RandomNum.Next(32) + 1
			};
			filesList.AddItem(string.Format(
				"{0}/{1}/{2}",
				files[i].month,
				files[i].day,
				files[i].year
			));
		}
	}
}

public partial class File : Node {
	public int day {get; init;}
	public int month {get; init;}
	public int year {get; init;}

	public bool EarlierThan(File otherFile) {
		if (year < otherFile.year) return true;
		if (year > otherFile.year) return false;
		if (month < otherFile.month) return true;
		if (month > otherFile.month) return false;
		if (day <= otherFile.day) return true;
		return false;
	}
}
