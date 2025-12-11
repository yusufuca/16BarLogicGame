using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    [Header("References")]
    public Transform target;

    [Header("Orbit Settings")]
    public float distance = 5.0f;
    public float minDistance = 2.0f; // Closest zoom
    public float maxDistance = 10.0f; // Furthest zoom
    public float zoomSensitivity = 2.0f;

    [Header("Input Settings")]
    public float mouseSensitivity = 4.0f;
    public Vector2 pitchLimits = new Vector2(-40, 85);

    [Header("Smoothing")]
    public float rotationSmoothTime = 0.12f;
    private Vector3 _rotationSmoothVelocity;
    private Vector3 _currentRotation;
    private float _yaw;
    private float _pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize rotation to match current camera to avoid snapping
        _currentRotation = transform.eulerAngles;
        _yaw = _currentRotation.y;
        _pitch = _currentRotation.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Rotation Input
        _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, pitchLimits.x, pitchLimits.y);

        // 2. Zoom Input (NEW LOGIC)
        // ScrollWheel returns approx +/- 0.1. We subtract to invert (Scroll Up = Zoom In/Decrease Distance)
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        distance -= scroll;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // 3. Smoothing
        Vector3 targetRotation = new Vector3(_pitch, _yaw);
        _currentRotation = Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationSmoothVelocity, rotationSmoothTime);

        // 4. Transform Calculation
        Quaternion finalRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 finalPosition = target.position + (finalRotation * negDistance);

        transform.rotation = finalRotation;
        transform.position = finalPosition;
    }
}