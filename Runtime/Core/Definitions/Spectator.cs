using UnityEngine;

namespace RealMethod
{
    public abstract class Spectator : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField]
        private float moveSpeed = 8f;
        [SerializeField]
        private float sprintSpeed = 20f;
        [SerializeField]
        private float lookSpeed = 0.15f;
        [Header("SpeedStep")]
        [SerializeField]
        private float speedStep = 2f;
        [SerializeField]
        private float minSpeed = 1f;
        [SerializeField]
        private float maxSpeed = 100f;
        [Header("Axis")]
        public bool _local = true;


        public float pitch { get; protected set; }
        public float yaw { get; protected set; }

        // Unity Methods
        protected virtual void Start()
        {
            Vector3 rot = _local ? transform.localRotation.eulerAngles : transform.eulerAngles;
            pitch = rot.x;
            yaw = rot.y;
            LockCursor(true);
        }

        // Methods
        protected virtual void UpdateLook(Vector2 lookInput)
        {
            yaw += lookInput.x * lookSpeed;
            pitch -= lookInput.y * lookSpeed;

            pitch = Mathf.Clamp(pitch, -90f, 90f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        protected virtual void UpdateMove(Vector3 move, bool sprint)
        {
            float speed = sprint ? sprintSpeed : moveSpeed;

            Vector3 direction = transform.TransformDirection(move.normalized);

            transform.position += direction * speed * Time.deltaTime;
        }
        protected virtual void LockCursor(bool enable)
        {
            Cursor.lockState = enable ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !enable;
        }

        protected virtual void ChangeSpeed(float delta)
        {
            moveSpeed = Mathf.Clamp(moveSpeed + delta * speedStep, minSpeed, maxSpeed);
            sprintSpeed = moveSpeed * 2.5f;
        }
    }
}