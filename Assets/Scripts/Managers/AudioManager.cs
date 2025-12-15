using Midevil.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
	// Editor Variables
	[Header("References")]
	public AudioSource musicSource;

	[Header("Music")]
	public AudioClip mainMenuMusic;
	public AudioClip darkForestMusic;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
			musicSource.PlayClip(mainMenuMusic);
		else
			musicSource.PlayClip(darkForestMusic);
	}
}
