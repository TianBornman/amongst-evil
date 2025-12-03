using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageNumberManager : Singleton<DamageNumberManager>
{
	// Editor Variables
	[Header("Settings")]
	public DamageNumber prefab;
	public int initialPool = 20;

	// Private Variables
	private readonly Queue<DamageNumber> pool = new();

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		pool.Clear();

		for (int i = 0; i < initialPool; i++)
			pool.Enqueue(Create());
	}

	// Public Methods
	public void ShowDamage(Vector3 worldPos, float amount, Color color)
	{
		var dmg = pool.Count > 0 ? pool.Dequeue() : Create();

		dmg.transform.position = worldPos;
		dmg.gameObject.SetActive(true);
		dmg.Setup(amount, color, () => pool.Enqueue(dmg));
	}

	// Private Methods
	private DamageNumber Create()
	{
		var obj = Instantiate(prefab);
		obj.gameObject.SetActive(false);

		return obj;
	}
}