using UnityEngine;

public enum WeaponType { Melee, Ranged }

public abstract class WeaponData : ScriptableObject
{
    public string weaponName = "New Weapon";
    public WeaponType weaponType;
    public float cooldown = 1f;
    public GameObject prefab; // weapon prefab or projectile

    public Sprite icon;
}
