using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [Header("Settings")]
    public float characterContactRadius = 1.2f;

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

        var partyMembers = PartyManager.Instance.PartyMembers;

        if (partyMembers == null)
            return;

        foreach (var member in partyMembers)
        {
            if (member == null || !member.IsAlive)
                continue;

            if (Vector3.Distance(transform.position, member.transform.position) <= characterContactRadius)
            {
                member.Damage(character, character.stats.damage, character.stats.critChance, character.stats.critDamage);
                cooldown = 1f / Mathf.Max(character.stats.attackSpeed, 0.1f);
                return;
            }
        }
    }
}
