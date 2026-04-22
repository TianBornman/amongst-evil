using System.Linq;
using UnityEngine;

public class PartyAutoShooter : MonoBehaviour
{
    [Header("References")]
    public Projectile projectilePrefab;

    [Header("Settings")]
    public Vector3 spawnOffset = new Vector3(0f, 1f, 0f);

    private Character character;
    private float cooldown;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (!character.IsAlive)
            return;

        cooldown -= Time.deltaTime;

        if (cooldown > 0)
            return;

        var target = SpawnManager.Instance.spawnedCharacters
            .Where(e => e != null && e.IsAlive)
            .OrderBy(e => Vector3.Distance(e.transform.position, transform.position))
            .FirstOrDefault();

        if (target == null)
            return;

        cooldown = 1f / Mathf.Max(character.stats.attackSpeed, 0.1f);
        Fire(target);
    }

    private void Fire(Character target)
    {
        Vector3 origin = transform.position + spawnOffset;
        Vector3 aimPoint = target.targetPos != null ? target.targetPos.position : target.transform.position;
        Vector3 dir = aimPoint - origin;

        if (dir == Vector3.zero)
            return;

        Quaternion rotation = Quaternion.LookRotation(dir.normalized);
        Projectile proj = Instantiate(projectilePrefab, origin, rotation);
        proj.Setup(character.stats, character);
    }
}
