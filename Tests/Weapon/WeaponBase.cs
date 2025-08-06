using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName = "Unnamed Weapon";
    public float cooldown = 1f;
    protected float lastUsedTime = -Mathf.Infinity;

    public virtual void Equip()
    {
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }

    public virtual bool CanUse()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void TryUse()
    {
        if (CanUse())
        {
            lastUsedTime = Time.time;
            Use();
        }
    }

    protected abstract void Use();
}
