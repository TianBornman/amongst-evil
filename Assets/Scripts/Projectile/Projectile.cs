using UnityEngine;

public class Projectile : MonoBehaviour
{
	// Public Variables
	public Stats stats;
	public Character owner;
	public float speed = 10f;

	// Public Methods
	public void Setup(Stats stats, Character owner)
	{
		this.stats = stats;
		this.owner = owner;
	}

	// Private Methods
	private void Awake()
	{
		Destroy(gameObject, 10);
	}

	private void Update()
	{
		transform.Translate(speed * Time.deltaTime * Vector3.forward);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<CharacterDetectionRange>() != null)
			return;

		Character hit = other.gameObject.GetComponentInParent<Character>();

		if (hit == null || hit == owner)
			return;

		if (owner.targetTeams.Contains(hit.team))
		{
			hit.Damage(owner, stats.damage, stats.critChance, stats.critDamage);
			Destroy(gameObject);
		}
	}
}
