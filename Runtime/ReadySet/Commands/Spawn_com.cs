using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Trigger/Spawn")]
    public class Spawn_com : ExecutCommand
    {
        [Header("Setting")]
        [SerializeField]
        private bool SelfPose = true;
        [Header("Dependency")]
        [SerializeField, ConditionalHide("SelfPose", true, false)]
        private Transform SpawnPoint;
        [Header("Assets")]
        [SerializeField]
        private UPrefab PrefabAsset;
        // ExecutCommand Methods
        protected override bool OnInitiate(Object author, Object owner)
        {
            return true;
        }
        protected override bool CanExecute(object Owner)
        {
            return enabled;
        }
        protected override void Execute(object Owner)
        {
            if (PrefabAsset == null)
                return;

            if (SelfPose)
            {
                Spawn.Prefab(PrefabAsset, transform.position, transform.rotation.eulerAngles);
            }
            else
            {
                Spawn.Prefab(PrefabAsset, transform.position, transform.rotation.eulerAngles);
            }

        }


    }
}