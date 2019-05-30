using UnityEngine;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour
{
    public SteamVR_Action_Vector2 joystickAction;
    public SteamVR_Input_Sources MoveHand;
    public SteamVR_Input_Sources RotateHand;
    public Rigidbody controllerRigidbody;
    public float MovementSpeed = 2.0f;
    public float MaxVelocity = 3;
    public float MinVelocity = -3;
    public float MaxFallSpeed = 35;
    public float RotationThreshhold = 0.7f;
    public float RotationSensitivity = 0.35f;

    private Vector3 movementMoving = Vector3.zero;
    private Vector3 movementRotation = Vector3.zero;

    void Start()
    {
    }

    void Update()
    {
        #region Movement
        Vector2 input1 = joystickAction.GetAxis(MoveHand);

        movementMoving = Vector3.zero;

        if (input1.y > 0.01f) movementMoving += transform.forward;
        if (input1.y < -0.01f) movementMoving += -transform.forward;
        if (input1.x > 0.01f) movementMoving += -transform.right;
        if (input1.x < -0.01f) movementMoving += transform.right;

        if ((input1.y > 0.01f && input1.x > 0.01f) || (input1.y < -0.01f && input1.x < -0.01f)) movementMoving.Normalize();

        controllerRigidbody.velocity += movementMoving * MovementSpeed;

        //Clamp the X, Y and Z axis velocities.
        controllerRigidbody.velocity = new Vector3
        (
            Mathf.Clamp
            (
                controllerRigidbody.velocity.x,
                MinVelocity,
                MaxVelocity
            ),

            Mathf.Clamp
            (
                controllerRigidbody.velocity.y,
                -MaxFallSpeed,
                MaxFallSpeed
            ),
            Mathf.Clamp
            (
                controllerRigidbody.velocity.z,
                MinVelocity,
                MaxVelocity
            )
        );
        #endregion

        #region Rotation
        Vector2 input2 = joystickAction.GetAxis(RotateHand);
        movementRotation = new Vector3(input2.x, 0, input2.y);
        Vector3 TargetPosition = transform.position + movementRotation;

        Vector3 directionalVector = TargetPosition - transform.position;

        bool checker = false;

        if (directionalVector.x > RotationThreshhold || directionalVector.x < -RotationThreshhold) checker = true;
        if (directionalVector.z > RotationThreshhold || directionalVector.z < -RotationThreshhold) checker = true;

        if (checker)
        {
            Quaternion rotation = Quaternion.LookRotation(directionalVector, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, RotationSensitivity);
        }
        #endregion
    }
}