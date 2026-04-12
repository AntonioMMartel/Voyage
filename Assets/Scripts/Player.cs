using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionReference pitch;
    [SerializeField] InputActionReference roll;
    [SerializeField] InputActionReference thrust;

    [Header("Movement")]
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float acceleration = 8f;
    private float currentSpeed;
    [SerializeField] float drag = 10f;

    [SerializeField] float fallSpeed = 15f;
    [SerializeField] float minFlightSpeed = 5f;
    private float verticalVelocity;

    [Header("Yaw")]
    [SerializeField] float turnSpeed = 2f;
    [SerializeField] float maxRollAngle = 60f;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 80f;

    [Header("Smoothing")]
    [SerializeField] float inputSmoothSpeed = 5f;
    private float pitchSmooth;
    private float yawSmooth;
    private float rollSmooth;
    private float thrustSmooth;




    private void OnEnable()
    {
        pitch.action.Enable();
        roll.action.Enable();
        thrust.action.Enable();
    }

    void OnDisable()
    {
        pitch.action.Disable();
        roll.action.Disable();
        thrust.action.Disable();
    }

    void Update()
    {
        float pitchInput = pitch.action.ReadValue<float>();
        float rollInput = roll.action.ReadValue<float>();
        float thrustInput = thrust.action.ReadValue<float>();

        pitchSmooth = Mathf.Lerp(pitchSmooth, pitchInput, Time.deltaTime * inputSmoothSpeed);
        rollSmooth = Mathf.Lerp(rollSmooth, rollInput, Time.deltaTime * inputSmoothSpeed);
        thrustSmooth = Mathf.Lerp(thrustSmooth, thrustInput, Time.deltaTime * inputSmoothSpeed);

        currentSpeed += thrustSmooth * acceleration * Time.deltaTime;
        if (thrustSmooth <= 0.01f)
        {
            currentSpeed -= drag * Time.deltaTime;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);


        Vector3 move = transform.forward * currentSpeed;
        //move.y += verticalVelocity;
        transform.Translate(move * Time.deltaTime, Space.World);

        if (Mathf.Abs(rollInput) < 0.1f) // Auto estabilización (me pone la nave recta)
        {
            rollSmooth = Mathf.Lerp(rollSmooth, 0f, Time.deltaTime * 2f);
        }

        // local
        transform.Rotate(
            -pitchSmooth * rotationSpeed * Time.deltaTime, // X (pitch)
             0,                                            // Y (yaw)
            -rollSmooth * rotationSpeed * Time.deltaTime   // Z (roll)
        );


        float rollAngle = transform.eulerAngles.z;
        if (rollAngle > 180f) rollAngle -= 360f; // Euler va de -180 a 180 y rotación de 0 a 360
        float newRoll = Mathf.Clamp(rollAngle, -maxRollAngle, maxRollAngle);
        float turnAmount = (newRoll / 45f) * turnSpeed;

        // world
        transform.Rotate(
            0f,
            (turnAmount * currentSpeed) * Time.deltaTime,
            0f,
            Space.World
        );

    }
}
