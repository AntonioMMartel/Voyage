using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionReference pitch;
    [SerializeField] InputActionReference yaw;
    [SerializeField] InputActionReference thrust;

    [Header("Movement")]
    public float maxSpeed = 20f;
    public float acceleration = 8f;
    private float currentSpeed;
    public float drag = 10f;

    [Header("Rotation")]
    public float rotationSpeed = 80f;

    [Header("Smoothing")]
    public float inputSmoothSpeed = 5f;

    private float pitchSmooth;
    private float yawSmooth;
    private float thrustSmooth;


    private void OnEnable()
    {
        pitch.action.Enable();
        yaw.action.Enable();
        thrust.action.Enable();

        pitch.action.started += PitchRotate;
        pitch.action.performed += PitchRotate;
        pitch.action.canceled += PitchRotate;
    }

    void OnDisable()
    {
        pitch.action.Disable();
        yaw.action.Disable();
        thrust.action.Disable();
    }

    private void PitchRotate(InputAction.CallbackContext context)
    {
        
    }
    void Update()
    {
        float pitchInput = pitch.action.ReadValue<float>();
        float yawInput = yaw.action.ReadValue<float>();
        float thrustInput = thrust.action.ReadValue<float>();

        Debug.Log(thrust.action.ReadValue<float>());


        pitchSmooth = Mathf.Lerp(pitchSmooth, pitchInput, Time.deltaTime * inputSmoothSpeed);
        yawSmooth = Mathf.Lerp(yawSmooth, yawInput, Time.deltaTime * inputSmoothSpeed);
        thrustSmooth = Mathf.Lerp(thrustSmooth, thrustInput, Time.deltaTime * inputSmoothSpeed);

        currentSpeed += thrustSmooth * acceleration * Time.deltaTime;
        if (thrustSmooth <= 0.01f)
        {
            currentSpeed -= drag * Time.deltaTime;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        transform.Rotate(
            -pitchSmooth * rotationSpeed * Time.deltaTime,
            0f,
             yawSmooth * rotationSpeed * Time.deltaTime
        );
    }
}
