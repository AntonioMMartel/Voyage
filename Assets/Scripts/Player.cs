using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 20f;
    public float acceleration = 5f;
    public float rotationSpeed = 60f;

    private float currentSpeed;

    void Update()
    {
        // Increase speed with Space
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, speed);

        // Move forward constantly
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Input axes
        float pitch = Input.GetAxis("Vertical");   // W/S or Up/Down
        float yaw = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float roll = Input.GetKey(KeyCode.Q) ? 1f : Input.GetKey(KeyCode.E) ? -1f : 0f;

        // Apply rotations
        transform.Rotate(-pitch * rotationSpeed * Time.deltaTime,
                          yaw * rotationSpeed * Time.deltaTime,
                          roll * rotationSpeed * Time.deltaTime);
    }
}
