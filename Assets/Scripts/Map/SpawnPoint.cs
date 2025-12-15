using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	// Editor Variables
	[Header("Settings")]
	public bool partySpawnPoint;

	// Public Methods
	public Vector3 GetSpawnPoint()
	{
		Vector3 from = transform.position;
		Vector3 direction = -transform.up;

		if (Physics.Raycast(from, direction, out RaycastHit hit, 40f))
		{
			return hit.point;
		}

		return -transform.up * 40f;
	}

	// Private Methods
	private void OnDrawGizmosSelected()
	{
		if (partySpawnPoint)
			Gizmos.color = Color.teal;
		else
			Gizmos.color = Color.orangeRed;

		Vector3 from = transform.position;
		Vector3 direction = -transform.up;

		if (Physics.Raycast(from, direction, out RaycastHit hit, 40f))
		{
			Gizmos.DrawLine(from, hit.point);
			Gizmos.DrawSphere(hit.point, 0.5f);
		}
		else
		{
			// Optional: show the ray even if it hits nothing
			Gizmos.DrawLine(from, from + direction * 40f);
		}
	}
}
