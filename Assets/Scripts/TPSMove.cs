using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class TPSMovement : MonoBehaviour
{
    [Header("Stats")]
    public float walkSpeed = 6;
    public float runSpeed = 10f;
    public float jumpHeight = 6f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform mainCamera; 
    public TPSCamera tpsCameraScript; // Need reference to Camera Script
    

    public float strafeTurnSpeed = 5.0f;

    private CharacterController _controller;
    private Animator _animator;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private float _currentSpeed;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _animator.applyRootMotion = false;

        if (mainCamera == null && Camera.main != null)
            mainCamera = Camera.main.transform;

        // Auto-find camera script if not assigned
        if (tpsCameraScript == null && mainCamera != null)
            tpsCameraScript = mainCamera.GetComponent<TPSCamera>();
    }

    void Update()
    {
               

        // Speed Control
        _currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 1. Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        // 2. Movement & Rotation Logic
        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            // LOCK-ON LOGIC CHECK
            if (tpsCameraScript != null && tpsCameraScript.IsLocked && tpsCameraScript.CurrentEnemyTarget != null)
            {
                // STRAFING: Look at Enemy, Move relative to camera
                Vector3 dirToEnemy = tpsCameraScript.CurrentEnemyTarget.position - transform.position;
                dirToEnemy.y = 0; // Don't look up/down
                
                Quaternion lookRot = Quaternion.LookRotation(dirToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * strafeTurnSpeed);

                // Movement Direction is still calculated from camera, but we don't rotate to it
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _controller.Move(moveDir.normalized * _currentSpeed * Time.deltaTime);
            }
            else
            {
                // FREE MOVEMENT: Look at movement direction
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _controller.Move(moveDir.normalized * _currentSpeed * Time.deltaTime);
            }
        }

        // 3. Gravity
        if (_controller.isGrounded && _velocity.y < 0) _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)
        {
            _velocity.y = jumpHeight;
            _controller.Move(_velocity * Time.deltaTime);
        }

        // 4. Animation
        float currentInputMagnitude = inputDir.magnitude;
        AudioManager.AMInstance.currentMagnitude = currentInputMagnitude;
        _animator.SetFloat("Speed", currentInputMagnitude, 0.1f, Time.deltaTime);
    }

    public void OnStep()
    {
        AudioManager.AMInstance.DetectSurface(transform.root);
    }
}