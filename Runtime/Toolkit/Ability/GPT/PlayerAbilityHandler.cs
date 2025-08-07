using UnityEngine;

public class PlayerAbilityHandler : MonoBehaviour
{
    public AbilityGPT[] abilities;
    public Transform aimTransform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseAbility(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseAbility(1);
    }

    void UseAbility(int index)
    {
        if (index < 0 || index >= abilities.Length) return;

        AbilityContext context = gameObject.AddComponent<AbilityContext>();
        context.target = GetTarget();
        context.targetPoint = GetTargetPoint();

        abilities[index].TryUse(gameObject, context);
    }

    GameObject GetTarget()
    {
        // Simplified raycast targeting
        if (Physics.Raycast(aimTransform.position, aimTransform.forward, out RaycastHit hit, 100f))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    Vector3 GetTargetPoint()
    {
        return aimTransform.position + aimTransform.forward * 10f;
    }

}
