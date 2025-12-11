using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TPSMovement : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 6.0f;
    public float turnSmoothTime = 0.1f; // How fast the mesh turns (seconds)
    public float gravity = -9.81f;

    [Header("References")]
    public Transform mainCamera; // Explicit reference to the camera

    // Internal State
    private CharacterController _controller;
    private float _turnSmoothVelocity; // Ref var for SmoothDamp
    private Vector3 _velocity; // For gravity calculation

    void Start()
    {
        _controller = GetComponent<CharacterController>();

        // Auto-assign camera if empty
        if (mainCamera == null)
        {
            if (Camera.main != null) mainCamera = Camera.main.transform;
            else Debug.LogError("No Main Camera found! Assign it manually.");
        }
    }

    void Update()
    {
        // 1. Input Polling (Raw prevents minimal filtering lag)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Normalize to prevent faster diagonal movement
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 2. Movement Logic (Only execute if input exists)
        if (direction.magnitude >= 0.1f)
        {
            // A. Calculate Target Angle
            // Atan2 gives the angle of the Input Vector (e.g., Pressing Right = 90 deg).
            // We ADD the Camera's Y rotation to make "Forward" relative to where we look.
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            // B. Smooth Rotation (Mesh Orientation)
            // Interpolate current rotation to target rotation.
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // C. Calculate Move Vector
            // Create a vector pointing in the direction of the TARGET ANGLE.
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // D. Apply Movement
            _controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // 3. Gravity Logic
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Reset downward velocity when grounded
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}