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

    private Rigidbody rb;

    [Header("Yaw")]
    [SerializeField] float turnSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] float pitchTorque = 50f;
    [SerializeField] float rollTorque = 50f;
    [SerializeField] float yawTorque = 20f;

    [Header("Smoothing")]
    [SerializeField] float inputSmoothSpeed = 5f;
    private float pitchInput, rollInput, thrustInput;
    private float pitchSmooth, yawSmooth, rollSmooth;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


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
        pitchInput = pitch.action.ReadValue<float>();
        rollInput = roll.action.ReadValue<float>();
        thrustInput = thrust.action.ReadValue<float>();
    }


    void FixedUpdate()
    {
        // Smooth input
        pitchSmooth = Mathf.Lerp(pitchSmooth, pitchInput, Time.fixedDeltaTime);
        //yawSmooth = Mathf.Lerp(yawSmooth, yawInput, Time.fixedDeltaTime * inputSmoothSpeed);
        rollSmooth = Mathf.Lerp(rollSmooth, rollInput, Time.fixedDeltaTime);

        // Speed control
        float targetSpeed = thrustInput * maxSpeed;
        currentSpeed += thrustInput * acceleration * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // Apply forward velocity
        rb.linearVelocity = transform.forward * currentSpeed;// + Vector3.up * rb.linearVelocity.y;


        // --- ROTATION ---

        // Direct torque (player control)
        Vector3 torque = new Vector3(
            -pitchSmooth * pitchTorque,
             0,
            -rollSmooth * rollTorque
        );

        rb.AddRelativeTorque(torque, ForceMode.Force);

        // --- TURN FROM ROLL (important) ---
        float rollAngle = transform.eulerAngles.z;
        if (rollAngle > 180f) rollAngle -= 360f;

        float turnAmount = (rollAngle / 45f) * turnSpeed;

        //rb.AddTorque(Vector3.up * turnAmount * currentSpeed, ForceMode.Force);
    }
}
