using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class TPSMovement : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform mainCamera;

    private CharacterController _controller;
    private Animator _animator;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        if (mainCamera == null && Camera.main != null)
            mainCamera = Camera.main.transform;

        // SAFETY: Ensure Root Motion is OFF via code to prevent mistakes
        _animator.applyRootMotion = false;
    }

    void Update()
    {
        // 1. Input Polling
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Create a direction vector strictly from Input
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        // 2. Movement Logic
        if (inputDir.magnitude >= 0.1f)
        {
            // Calculate rotation relative to camera
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);

            // Rotate
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // 3. Gravity
        if (_controller.isGrounded && _velocity.y < 0) _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        // 4. Animation Logic (CHANGED TO INPUT)
        // Instead of asking "How fast am I moving?", we ask "How much am I pressing the keys?"
        // This breaks the deadlock.
        float currentInputMagnitude = inputDir.magnitude;

        // Send exactly 0.0 or 1.0 based on key press
        _animator.SetFloat("Speed", currentInputMagnitude, 0.1f, Time.deltaTime);
    }
}