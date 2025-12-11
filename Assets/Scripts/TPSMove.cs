using UnityEngine;

[RequireComponent(typeof(CharacterController))]
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

        // DEBUG 1: Try to find the animator and print the result
        _animator = GetComponentInChildren<Animator>();

        if (_animator != null)
        {
            Debug.Log("SUCCESS: Found Animator on object: " + _animator.gameObject.name);
        }
        else
        {
            Debug.LogError("FAILURE: Could not find any Animator component in children! Check Hierarchy.");
        }

        if (mainCamera == null)
        {
            if (Camera.main != null) mainCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        // ... (Movement Logic Same as Before) ...
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        if (_controller.isGrounded && _velocity.y < 0) _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        // DEBUG SECTION
        if (_animator != null)
        {
            Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z);
            float speedPercent = horizontalVelocity.magnitude / moveSpeed;
            speedPercent = Mathf.Clamp01(speedPercent);

            // DEBUG 2: Print the value we are trying to send
            // Only print if moving to avoid spamming console
            if (speedPercent > 0.01f)
            {
                Debug.Log("Sending Speed Value: " + speedPercent);
            }

            _animator.SetFloat("Speed", speedPercent, 0.15f, Time.deltaTime);
        }
    }
}