using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // LOGIC: Rotate this object to look at the camera
        // We add 180 degrees because UI elements face 'backwards' by default in World Space
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                         _mainCamera.transform.rotation * Vector3.up);
    }
}