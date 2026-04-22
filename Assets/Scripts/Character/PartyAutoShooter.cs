using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyAutoShooter : MonoBehaviour
{
    [Header("References")]
    public Projectile projectilePrefab;

    [Header("Settings")]
    public Vector3 spawnOffset = new Vector3(0f, 1f, 0f);

    // How many of the nearest enemies to consider when spreading fire.
    // Each party member picks randomly from this pool so they don't all lock onto the same target.
    [SerializeField] private int targetPoolSize = 3;

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

        Character target = PickTarget();

        if (target == null)
            return;

        cooldown = 1f / Mathf.Max(character.stats.attackSpeed, 0.1f);
        Fire(target);
    }

    private Character PickTarget()
    {
        float range = character.stats.range;

        List<Character> inRange = SpawnManager.Instance.spawnedCharacters
            .Where(e => e != null && e.IsAlive &&
                   Vector3.Distance(e.transform.position, transform.position) <= range)
            .OrderBy(e => Vector3.Distance(e.transform.position, transform.position))
            .ToList();

        if (inRange.Count == 0)
            return null;

        // Pick randomly from the closest N so party members spread their fire
        int pickFrom = Mathf.Min(targetPoolSize, inRange.Count);
        return inRange[Random.Range(0, pickFrom)];
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
