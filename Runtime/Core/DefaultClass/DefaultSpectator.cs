using RealMethod;
using UnityEngine;
// #if ENABLE_INPUT_SYSTEM
// using UnityEngine.InputSystem;
// #endif

public sealed class DefaultSpectator : Spectator
{
// #if ENABLE_INPUT_SYSTEM
//     public InputActionReference scrollWheel;
// #endif

    // Unity Events
    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandelStepSpeed();
    }

    // Methods
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        UpdateLook(new Vector2(mouseX, mouseY));
    }
    private void HandleMovement()
    {
        bool sprint = Input.GetKey(KeyCode.LeftShift);

        Vector3 move = Vector3.zero;

        move.x = (Input.GetKey(KeyCode.D) ? 1 : 0) -
         (Input.GetKey(KeyCode.A) ? 1 : 0);

        move.y = (Input.GetKey(KeyCode.E) ? 1 : 0) -
                 (Input.GetKey(KeyCode.Q) ? 1 : 0);

        move.z = (Input.GetKey(KeyCode.W) ? 1 : 0) -
                 (Input.GetKey(KeyCode.S) ? 1 : 0);

        UpdateMove(move.normalized, sprint);
        //transform.position += move * speed * Time.deltaTime;
    }
    private void HandelStepSpeed()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            ChangeSpeed(scroll);
        }

        // Vector2 scroll = scrollWheel.action.ReadValue<Vector2>();

        // if (Mathf.Abs(scroll.y) > 0.01f)
        // {
        //     ChangeSpeed(scroll.y);
        // }
    }

}