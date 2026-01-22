using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    [Header("References")]
    public Transform target;

    [Header("Orbit Settings")]
    public float distance = 5.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    public float zoomSensitivity = 2.0f;

    [Header("Input Settings")]
    public float mouseSensitivity = 4.0f;
    public Vector2 pitchLimits = new Vector2(-40, 85);

    [Header("Lock-On System")]
    public float lockOnRange = 20f;
    public float lockOnHeightOffset = 1.0f;
    public float lockOnDamping = 0.1f;
    public LayerMask obstacleLayers;
    public string enemyTag = "Enemy";
    public string bossTag = "Boss";

    // Public properties for Movement Script
    public bool IsLocked { get; private set; }
    public Transform CurrentEnemyTarget { get; private set; }

    // Internals
    private float _rotationSmoothTime = 0.12f;
    private Vector3 _rotationSmoothVelocity;
    private Vector3 _currentRotation;
    private float _lockOnYawVelocity;
    private float _yaw;
    private float _pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _currentRotation = transform.eulerAngles;
        _yaw = _currentRotation.y;
        _pitch = _currentRotation.x;
    }

    void Update()
    {
        // Right Mouse Button for Lock-On
        if (Input.GetMouseButtonDown(1))
        {
            
          LockTarget();
        }
        else if(Input.GetMouseButtonUp(1))
        {
            UnlockTarget();
        }
        // Safety: Unlock if enemy dies/vanishes
        if (IsLocked && CurrentEnemyTarget == null) UnlockTarget();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- 1. Rotation Logic ---
        if (IsLocked && CurrentEnemyTarget != null)
        {
            // Calculate vector to enemy
            Vector3 dirToEnemy = (CurrentEnemyTarget.position) - target.position;
            Quaternion lookRotation = Quaternion.LookRotation(dirToEnemy);

            // ONLY update YAW (Horizontal). 
            // We ignore Pitch so you can still look up/down manually.
            float targetYaw = lookRotation.eulerAngles.y;
            _yaw = Mathf.SmoothDampAngle(_yaw, targetYaw, ref _lockOnYawVelocity, lockOnDamping);

            // Optional: Allow manual fine-tuning even while locked?
            // Remove this line if you want strict lock:
            _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }
        else
        {
            // Free Look
            _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        // Clamp Pitch (Applied to both states)
        _pitch = Mathf.Clamp(_pitch, pitchLimits.x, pitchLimits.y);

        // --- 2. Zoom & Smooth ---
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 targetRotation = new Vector3(_pitch, _yaw);

        // SmoothDamp handles the 360 wrap-around automatically via Vector3
        _currentRotation = Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationSmoothVelocity, _rotationSmoothTime);

        // --- 3. Apply Transform ---
        Quaternion finalRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
        Vector3 negDistance = new Vector3(0, 0, -distance);

        transform.rotation = finalRotation;
        transform.position = target.position + (finalRotation * negDistance);
    }

    void LockTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(target.position, lockOnRange);
        Transform nearestEnemy = null;
        float minAngle = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag(enemyTag) || col.CompareTag(bossTag))
            {
                // Pick enemy closest to screen center
                Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                if (angle < minAngle && angle < 60)
                {
                    minAngle = angle;
                    nearestEnemy = col.transform; 
                }
            }
        }

        if (nearestEnemy != null)
        {
            CurrentEnemyTarget = nearestEnemy;
            IsLocked = true;
        }
    }

    void UnlockTarget()
    {
        IsLocked = false;
        CurrentEnemyTarget = null;
    }
}