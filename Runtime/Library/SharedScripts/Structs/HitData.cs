using UnityEngine;

namespace RealMethod
{
    public struct HitData
    {
        // Actors
        public GameObject attacker;
        public GameObject target;

        // Components
        public Collider collider;
        public Rigidbody rigidbody;

        // Location
        public Vector3 point;          // Impact point
        public Vector3 normal;         // Impact normal
        public Vector3 traceStart;     // Trace start
        public Vector3 traceEnd;       // Trace end

        // Direction
        public Vector3 direction;

        // Distance
        public float distance;

        // Damage
        public float damage;

        // Surface info
        public PhysicsMaterial physicMaterial;

        // State
        public bool blockingHit;
        public bool startPenetrating;

        // Utility constructor
        public HitData(RaycastHit hit, GameObject attacker, float damage)
        {
            this.attacker = attacker;
            this.target = hit.collider ? hit.collider.gameObject : null;

            collider = hit.collider;
            rigidbody = hit.rigidbody;

            point = hit.point;
            normal = hit.normal;

            traceStart = Vector3.zero;
            traceEnd = Vector3.zero;

            direction = Vector3.zero;

            distance = hit.distance;

            this.damage = damage;

            physicMaterial = hit.collider ? hit.collider.sharedMaterial : null;

            blockingHit = true;
            startPenetrating = false;
        }
    }

}