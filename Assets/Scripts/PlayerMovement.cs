using UnityEngine;

// REQUIREMENT: This script forces Unity to add a CharacterController component if missing.
[RequireComponent(typeof(CharacterController))]
public class SimpleMove : MonoBehaviour
{
    [Header("Locomotion Settings")]
    // Speed in units per second
    public float moveSpeed = 6.0f;
    // Downward acceleration for gravity
    public float gravity = -9.81f;

    [Header("References")]
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;

    void Start()
    {
        // CACHING: Get the reference to the component on this GameObject once at startup.
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // STEP 1: Ground Check
        // The CharacterController has a built-in boolean to check if it's touching the floor.
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _velocity.y < 0)
        {
            // Reset vertical velocity to a small value to ensure we stick to the ground.
            _velocity.y = -2f;
        }

        // STEP 2: Input Polling
        // Get scalar values (-1 to 1) from WASD or Arrow Keys.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // STEP 3: Vector Construction (Local Space)
        // transform.right corresponds to the Red Axis (X).
        // transform.forward corresponds to the Blue Axis (Z).
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        // STEP 4: Application (Horizontal Movement)
        // We multiply Direction * Speed * DeltaTime (time since last frame).
        _controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // STEP 5: Gravity Logic
        // Apply constant downward acceleration to the Y axis.
        _velocity.y += gravity * Time.deltaTime;

        // STEP 6: Application (Vertical Movement)
        _controller.Move(_velocity * Time.deltaTime);
    }
}