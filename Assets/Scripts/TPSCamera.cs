using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    [Header("References")]
    // The empty GameObject attached to the Player's head
    public Transform target;

    [Header("Orbit Settings")]
    public float distance = 5.0f; // Radius of the sphere
    public float mouseSensitivity = 4.0f;

    // Limits vertical look to prevent flipping (e.g., -40 floor, 85 sky)
    public Vector2 pitchLimits = new Vector2(-40, 85);

    [Header("Smoothing")]
    // Time in seconds for the camera to reach the target rotation
    public float rotationSmoothTime = 0.12f;
    private Vector3 _rotationSmoothVelocity; // Reference var for SmoothDamp

    // Internal Euler angles (Degrees)
    private Vector3 _currentRotation;
    private float _yaw;   // Horizontal (Y axis)
    private float _pitch; // Vertical (X axis)

    void Start()
    {
        // Initialization: Remove cursor from screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // LATEUPDATE: Runs after Update(). 
    // Critical for cameras to prevent jitter if the Player moves in Update().
    void LateUpdate()
    {
        if (target == null) return;

        // STEP 1: Input Accumulation
        // Read raw mouse delta.
        _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity; // Subtract to invert Y (Standard)

        // STEP 2: Clamping
        // Restrict the vertical angle so we can't look between our own legs.
        _pitch = Mathf.Clamp(_pitch, pitchLimits.x, pitchLimits.y);

        // STEP 3: Smoothing (Mathf.SmoothDamp)
        // This interpolates the value to prevent jerky camera movement.
        Vector3 targetRotation = new Vector3(_pitch, _yaw);
        _currentRotation = Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationSmoothVelocity, rotationSmoothTime);

        // STEP 4: Quaternion Calculation
        // Convert the smoothed Vector3 (Euler) into a Rotation (Quaternion).
        Quaternion finalRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);

        // STEP 5: Position Calculation (The Orbit Logic)
        // Logic: Start at Target -> Rotate to Angle -> Move Backwards by Distance
        // Vector3.forward is (0,0,1). Multiplying by negative distance gives (0,0,-5).
        // Multiplying by Rotation applies the angle to that offset.
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 finalPosition = target.position + (finalRotation * negDistance);

        // STEP 6: Apply Transforms
        transform.rotation = finalRotation;
        transform.position = finalPosition;
    }
}