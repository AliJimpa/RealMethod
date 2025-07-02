using System;
using System.Collections;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Trigger/Fracture")]
    public sealed class Fracture_com : ExecutCommand
    {
        [Header("Fracture")]
        [SerializeField]
        private GameObject HealthyObject;
        [SerializeField]
        private GameObject DamagedObject;
        [Header("Setting")]
        [SerializeField, Range(1f, 10f)]
        private float resistanceFactor = 1.0f;
        [SerializeField]
        private float explosionForce = 40;
        [SerializeField, Range(0f, 1f)]
        private float VelocityAffection = 1;
        [SerializeField]
        private float VelocityForceMultiplier = 1;
        [Space]
        [SerializeField]
        private float dissolvetime = 8;
        [Header("َAdvance")]
        [SerializeField, ReadOnly]
        private Rigidbody[] choppedBodies;
        [SerializeField, ReadOnly]
        private Pose[] FractionPoses;


        // ExecutCommand Methods
        protected override bool OnInitiate(UnityEngine.Object author, UnityEngine.Object owner)
        {
            if (HealthyObject)
                HealthyObject.SetActive(true);
            if (DamagedObject)
                DamagedObject.SetActive(false);
            return true;
        }
        protected override bool CanExecute(object Owner)
        {
            return enabled;
        }
        protected override void Execute(object Owner)
        {
            if (Owner is Collider col)
            {
                Fracture(col);
                return;
            }

            if (Owner is Transform trans)
            {
                Fracture(trans);
            }
        }




        private void OnValidate()
        {
            // Find HealthyObject and DamagedObject in children
            foreach (Transform child in transform)
            {
                if (child.name == "HealthyObject")
                    HealthyObject = child.gameObject;
                else if (child.name == "DamagedObject")
                    DamagedObject = child.gameObject;
            }

            // Ensure HealthyObstacle and DamagedObstacle are assigned
            if (!HealthyObject || !DamagedObject)
                return;

            // Initialize Rigidbodies
            choppedBodies = DamagedObject.GetComponentsInChildren<Rigidbody>(true);

            // Save Fraction Poses for Resetting Obstacle
            FractionPoses = new Pose[choppedBodies.Length];
            for (int i = 0; i < choppedBodies.Length; i++)
            {
                FractionPoses[i].position = choppedBodies[i].transform.localPosition;
                FractionPoses[i].rotation = choppedBodies[i].transform.localRotation;
            }
        }


        public GameObject Fracture(Transform tranfome)
        {
            PreFracture();
            foreach (Rigidbody rb in choppedBodies)
            {
                AddCustomForce(rb, explosionForce, tranfome.position);
            }
            StartCoroutine(Dissolve());
            return GetDamagedObject();
        }
        public GameObject Fracture(Collider other)
        {
            PreFracture();

            // ArtificialEnergy
            Rigidbody OtherPhysic = other.gameObject.GetComponent<Rigidbody>();
            float OtherSpeed = OtherPhysic != null ? OtherPhysic.linearVelocity.magnitude : 1;

            // Find Hit Position
            Vector3 explosionPosition = other.ClosestPoint(transform.position);

            // PhysicForce
            foreach (Rigidbody rb in choppedBodies)
            {
                float finalExplosionForce = Mathf.Lerp(explosionForce, OtherSpeed * VelocityForceMultiplier, VelocityAffection);
                AddCustomForce(rb, finalExplosionForce, explosionPosition);
            }
            StartCoroutine(Dissolve());
            return GetDamagedObject();
        }
        public GameObject GetHealthyObject()
        {
            return HealthyObject;
        }
        public GameObject GetDamagedObject()
        {
            return DamagedObject;
        }


        private void AddCustomForce(Rigidbody rb, float Force, Vector3 Position)
        {
            rb.AddExplosionForce(Force / resistanceFactor, Position, 20.0f);
        }
        private void PreFracture()
        {
            HealthyObject.SetActive(false);
            DamagedObject.SetActive(true);
        }

        private IEnumerator Dissolve()
        {
            yield return new WaitForSeconds(dissolvetime);
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
            float elapsedTime = 0f;

            while (elapsedTime < 2.0f)
            {
                elapsedTime += Time.deltaTime;
                transform.position -= new Vector3(0, 2 * Time.deltaTime, 0);
                yield return null; // Wait for the next frame
            }

            for (int i = 0; i < choppedBodies.Length; i++)
            {
                choppedBodies[i].isKinematic = true;
            }
            DamagedObject.SetActive(false);
        }




    }
}