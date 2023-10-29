using Godot;
using System;
using System.Diagnostics;

/// <summary>
/// Both holds data about a task and acts as the task "window".
/// </summary>
public partial class DatesTask : Task
{
	private int selected = -1;

	private int filesLength;
	private File[] files;
	[Export]
	private ItemList filesList;

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (!Visible)
			return;

		if (filesList.IsAnythingSelected()) {
			int newSelected = filesList.GetSelectedItems()[0];
			if (selected == -1) {
				selected = newSelected;
			} else if (selected != newSelected) {
				filesList.DeselectAll();
				File swapFile = files[selected];
				String swapString = filesList.GetItemText(selected);
				files[selected] = files[newSelected];
				filesList.SetItemText(selected, filesList.GetItemText(
					newSelected));
				files[newSelected] = swapFile;
				filesList.SetItemText(newSelected, swapString);
				selected = -1;
			}
		} else {
			selected = -1;
		}
	}

	public void Submit() {
		for (int i = 0; i < filesLength - 1; i++) {
			if (!files[i].EarlierThan(files[i + 1])) {
				Fail();
				return;
			}
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
		filesLength = max_files;
		int baseYear = RandomNum.Next(3001);
		for (int i = 0; i < max_files; i++) {
			files[i] = new File() {
				year = RandomNum.Next(1000) + baseYear,
				month = RandomNum.Next(13) + 1,
				day = RandomNum.Next(32) + 1
			};
			filesList.AddItem(string.Format(
				"{0} / {1} / {2}",
				files[i].month,
				files[i].day,
				files[i].year
			));
		}
	}
}

public class File {
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
