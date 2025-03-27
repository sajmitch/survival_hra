using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform; 
    public float parallaxEffect = 0.5f; // intenzita parallax efektu

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // kdyžtak si veme main cameru
        }
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, deltaMovement.y * parallaxEffect, 0);
        lastCameraPosition = cameraTransform.position;
    }
}