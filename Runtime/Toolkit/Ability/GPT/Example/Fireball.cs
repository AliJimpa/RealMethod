using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float damage = 20f;

    private void OnCollisionEnter(Collision other)
    {
        // Health health = other.collider.GetComponent<Health>();
        // if (health != null)
        // {
        //     health.TakeDamage(damage);
        // }

        Destroy(gameObject);
    }
}

