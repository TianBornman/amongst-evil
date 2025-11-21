using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HubManager : Singleton<HubManager>
{
	// Private Variables
	private MainMenuCamera mainMenuCamera;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != 0)
			return;

				mainMenuCamera = FindFirstObjectByType<MainMenuCamera>();
	}

	// Public Methods
	public void StartGame()
	{
		HubUiManager.Instance.ShowRecruitmentUI();
		mainMenuCamera.MoveToHub();
	}
}
