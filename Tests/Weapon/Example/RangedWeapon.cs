using UnityEngine;
using System.Collections;

public class RangedWeapon : WeaponBase
{
    private RangedWeaponData rangedData;
    private int currentAmmo;
    private bool isReloading = false;

    // public override void Initialize(WeaponData newData)
    // {
    //     base.Initialize(newData);
    //     rangedData = (RangedWeaponData)newData;
    //     currentAmmo = rangedData.maxAmmo;
    // }

    protected override void Use()
    {
        if (isReloading || currentAmmo <= 0)
        {
            if (!isReloading) StartCoroutine(Reload());
            return;
        }

        currentAmmo--;

        GameObject proj = Instantiate(rangedData.prefab, transform.position, transform.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = transform.forward * rangedData.projectileSpeed;

        //WeaponUI.Instance?.UpdateAmmo(currentAmmo, rangedData.maxAmmo);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(rangedData.reloadTime);
        currentAmmo = rangedData.maxAmmo;
        isReloading = false;

        //WeaponUI.Instance?.UpdateAmmo(currentAmmo, rangedData.maxAmmo);
    }
}
