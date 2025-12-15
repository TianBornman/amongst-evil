using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	public float baseIntensity = 2f;
	public float flickerStrength = 0.6f;
	public float flickerSpeed = 8f;

	private Light fireLight;
	private float seed;

	void Awake()
	{
		fireLight = GetComponent<Light>();
		seed = Random.value * 100f;
	}

	void Update()
	{
		float noise = Mathf.PerlinNoise(seed, Time.time * flickerSpeed);
		fireLight.intensity = baseIntensity + (noise - 0.5f) * flickerStrength;
	}
}
