using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	// Private Variables
	private bool atMenu = true;
	private bool atHub = true;
	private bool isGamePaused = false;
	private string mapName = "Dark Forest";

	// Public Properties
	public bool AtMainMenu { get => atMenu; }
	public bool AtHub { get => atHub; }
	public bool IsGamePaused { get => isGamePaused; }
	public string MapName { get => mapName; }

	// Public Methods
	public void LeaveMenu()
	{
		atMenu = false;
	}

	public void EnterHub()
	{
		atHub = true;
	}

	public void LeaveHub()
	{
		atHub = false;
	}

	public void PauseGame()
	{
		isGamePaused = true;
		Time.timeScale = 0f;
	}

	public void ResumeGame()
	{
		isGamePaused = false;
		Time.timeScale = 1f;
	}
}
