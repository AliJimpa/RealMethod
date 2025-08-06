using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fireball")]
public class FireballAbility : Ability
{
    public GameObject fireballPrefab;
    public float projectileSpeed = 10f;
    public float damage = 25f;

    public override void Activate(GameObject user, AbilityContext context)
    {
        Transform spawnPoint = user.transform;
        GameObject fireball = Instantiate(fireballPrefab, spawnPoint.position + spawnPoint.forward, spawnPoint.rotation);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        rb.linearVelocity = spawnPoint.forward * projectileSpeed;

        Fireball fb = fireball.GetComponent<Fireball>();
        if (fb != null)
        {
            fb.damage = damage;
        }

        // Optional VFX/SFX
        if (vfxPrefab) Instantiate(vfxPrefab, user.transform.position, Quaternion.identity);
        if (sfxClip) AudioSource.PlayClipAtPoint(sfxClip, user.transform.position);
    }
}
