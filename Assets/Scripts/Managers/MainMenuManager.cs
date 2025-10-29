using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public UIDocument mainMenuUi;

	// Private Methods
	private void Start()
	{
		// Bind the buttons
		var startButton = mainMenuUi.rootVisualElement.Q<Button>("Play");
		startButton.clicked += () => SceneManager.LoadScene("Level");
	}
}
