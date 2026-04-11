using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 20f;
    public float acceleration = 5f;
    public float rotationSpeed = 60f;

    private float currentSpeed;

    [SerializeField] InputActionReference pitch; // Arriba abajo
    [SerializeField] InputActionReference yaw; // Izquierda derecha
    [SerializeField] InputActionReference thrust;


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

        transform.Rotate(
            -pitchInput * rotationSpeed * Time.deltaTime,
            0,
             yawInput * rotationSpeed * Time.deltaTime
             );

        transform.Translate(Vector3.forward * thrustInput * speed * Time.deltaTime);
    }
}
