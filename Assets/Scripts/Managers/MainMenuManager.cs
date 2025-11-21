using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public UIDocument mainMenuUi;

	// Private Variables
	private MainMenuCamera mainMenuCamera;

	// Private Methods
	private void Awake()
	{
		mainMenuCamera = FindFirstObjectByType<MainMenuCamera>();
	}

	private void Start()
	{
		// Bind the buttons
		var startButton = mainMenuUi.rootVisualElement.Q<Button>("Play");
		startButton.clicked += StartGame;
		//startButton.clicked += () => SceneManager.LoadScene("Level");
	}

	private void StartGame()
	{
		mainMenuUi.rootVisualElement.visible = false;
		mainMenuCamera.MoveToHub();
	}
}
