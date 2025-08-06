using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    public float damage = 25f;
    public float range = 2f;

    protected override void Use()
    {
        // Basic raycast melee attack
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range))
        {
            var health = hit.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        Debug.Log($"{weaponName} used for melee attack");
    }
}
