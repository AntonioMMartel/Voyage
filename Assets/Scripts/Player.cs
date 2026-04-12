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

    [SerializeField] float rollDampen = 5f;
    [SerializeField] float pitchDampen = 1f;
    [SerializeField] float thrustDampen = 2f;

    [Header("Gravity")]
    [SerializeField] float flightSpeedThreshold= 10f;
    [SerializeField] float gravityMultiplier = 1f; // base gravity strength
    [SerializeField] float pitchStallTorque = 20f;

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


        // cuando dejas de pulsa desacelera
        float accelRate = (thrustInput > 0.01f) ? acceleration : -thrustDampen;


        // Speed control
        float targetSpeed = thrustInput * maxSpeed;
        currentSpeed += accelRate * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);


        // Apply forward velocity
        rb.linearVelocity = transform.forward * currentSpeed;

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


        // Estabilización

        Vector3 localAngular = transform.InverseTransformDirection(rb.angularVelocity);

        // Strong damping on roll when no input
        if (Mathf.Abs(rollInput) < 0.01f)
        {
            localAngular.z = Mathf.Lerp(localAngular.z, 0f, Time.fixedDeltaTime * rollDampen);
        }

        // Optional: mild damping on pitch too
        if (Mathf.Abs(pitchInput) < 0.01f)
        {
            localAngular.x = Mathf.Lerp(localAngular.x, 0f, Time.fixedDeltaTime * pitchDampen);
        }

        rb.angularVelocity = transform.TransformDirection(localAngular);

        
        // Gravity

        float gravityFactor = 1f - Mathf.InverseLerp(0f, minFlightSpeed, currentSpeed);

        Vector3 gravityForce = Physics.gravity * gravityMultiplier * gravityFactor;

        // Pitch hacia delante al caer

        // Prevenimos que interrumpa input
        bool hasPitchInput = Mathf.Abs(pitchInput) > 0.01f;
        bool hasRollInput = Mathf.Abs(rollInput) > 0.01f;
        bool hasInput = hasPitchInput || hasRollInput;

        float pitchAngle = transform.eulerAngles.x;
        if (pitchAngle > 180f) pitchAngle -= 360f;

        // Al final si paras el motor te caes girando sin control que mola y se ve realista
        if (pitchAngle < 80f & !hasInput) // pitchAngle > ángulo máximo de caída
        {
            float stallPitchForce = gravityFactor * pitchStallTorque;
            rb.AddRelativeTorque(Vector3.right * stallPitchForce, ForceMode.Acceleration);
        }
        rb.AddForce(gravityForce, ForceMode.Acceleration);
    }
}
