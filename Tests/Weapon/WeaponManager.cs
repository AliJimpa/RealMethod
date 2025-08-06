using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public WeaponBase[] weapons;
    private int currentIndex = 0;
    private WeaponBase currentWeapon;

    void Start()
    {
        EquipWeapon(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);

        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            currentWeapon?.TryUse();
        }
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        if (currentWeapon != null)
        {
            currentWeapon.Unequip();
        }

        currentIndex = index;
        currentWeapon = weapons[index];
        currentWeapon.Equip();
    }
}
