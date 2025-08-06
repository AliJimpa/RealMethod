using UnityEngine;
using RealMethod;

public enum AbilityTargetType { Self, Enemy, Area }
public enum AbilityResourceType { None, Mana, Stamina }

public abstract class Ability : ScriptableObject
{
    public string abilityName = "New Ability";
    public float cooldown = 2f;
    public float castTime = 0f;

    [Header("Targeting")]
    public AbilityTargetType targetType = AbilityTargetType.Self;

    [Header("Resource Cost")]
    public AbilityResourceType resourceType = AbilityResourceType.None;
    public float resourceCost = 0f;

    public GameObject vfxPrefab;
    public AudioClip sfxClip;

    protected float lastUsedTime = -Mathf.Infinity;

    // public virtual bool CanUse(GameObject user, IConsumableResource resourceUser)
    // {
    //     bool resourceAvailable = resourceType == AbilityResourceType.None ||
    //         (resourceUser != null && resourceUser.CanConsume(resourceCost));

    //     return Time.time >= lastUsedTime + cooldown && resourceAvailable;
    // }

    // public void TryUse(GameObject user, AbilityContext context)
    // {
    //     IConsumableResource resourceUser = user.GetComponent<IConsumableResource>();
    //     if (CanUse(user, resourceUser))
    //     {
    //         lastUsedTime = Time.time;

    //         if (resourceType != AbilityResourceType.None)
    //         {
    //             resourceUser?.Consume(resourceCost);
    //         }

    //         if (castTime > 0)
    //         {
    //             context.StartCoroutine(CastCoroutine(user, context));
    //         }
    //         else
    //         {
    //             Activate(user, context);
    //         }
    //     }
    // }

    private System.Collections.IEnumerator CastCoroutine(GameObject user, AbilityContext context)
    {
        yield return new WaitForSeconds(castTime);
        Activate(user, context);
    }

    public abstract void Activate(GameObject user, AbilityContext context);
}
