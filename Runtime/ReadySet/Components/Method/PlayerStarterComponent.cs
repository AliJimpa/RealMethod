using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Method/PlayerStarter")]
    public class PlayerStarterComponent : PlayerStarter
    {
        [Header("Debug")]
        [SerializeField]
        private float height = 2f;
        [SerializeField]
        private float radius = 0.5f;


        // Unity Methods
        private void Awake()
        {
            Destroy(gameObject);
        }


#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            RM_Draw.gizmos.Capsule(transform.position, Color.cyan, height, radius);
            RM_Draw.gizmos.Arrow(transform.position, transform.forward, Color.red);
            RM_Draw.gizmos.Text(PosName, transform.position + (transform.up * (height / 2)) + (Vector3.up * 0.1f), Color.black);
        }
#endif
    }
}