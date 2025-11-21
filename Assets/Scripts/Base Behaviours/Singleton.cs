using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this as T;
		DontDestroyOnLoad(gameObject);
	}

	protected virtual void OnEnable()
	{
		SceneManager.sceneLoaded += HandleSceneLoaded;
	}

	protected virtual void OnDisable()
	{
		SceneManager.sceneLoaded -= HandleSceneLoaded;

		if (Instance == this)
			Instance = null;
	}

	private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		OnSceneLoaded(scene, mode);
	}

	protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
}