using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Ranged Weapon")]
public class RangedWeaponData : WeaponData
{
    public float projectileSpeed = 10f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
}
