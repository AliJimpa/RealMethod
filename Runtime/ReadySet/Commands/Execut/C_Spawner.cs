using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Execut/Spawner")]
    public sealed class C_Spawner : Command
    {
        [Header("Setting")]
        private bool AutoAttach = false;
        [SerializeField]
        private bool SelfPose = true;
        [SerializeField, ConditionalHide("SelfPose", true, true)]
        private Transform SpawnPoint;
        [Header("Resource")]
        [SerializeField]
        private Prefab PrefabAsset;
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

            if (AutoAttach)
            {
                if (Owner is MonoBehaviour Mono)
                {
                    GameObject target = Spawn.Prefab(PrefabAsset, Mono.transform);
                    if (!SelfPose)
                    {
                        target.transform.position = SpawnPoint.position;
                        target.transform.rotation = SpawnPoint.rotation;
                    }
                }
                else
                {
                    Debug.LogError($"This command ({nameof(C_Spawner)}) should Execute by Monobehavior for AutoAttach");
                }
            }
            else
            {
                if (SelfPose)
                {
                    Spawn.Prefab(PrefabAsset, transform.position, transform.rotation.eulerAngles);
                }
                else
                {
                    Spawn.Prefab(PrefabAsset, SpawnPoint.position, SpawnPoint.rotation.eulerAngles);
                }
            }
        }


    }
}