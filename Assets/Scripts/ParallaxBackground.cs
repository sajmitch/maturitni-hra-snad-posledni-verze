using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;  // Reference na kameru
    public float parallaxEffect = 0.5f; // Intenzita parallax efektu (nižší = pomalejší pohyb)

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // Pokud není nastaveno, použije hlavní kameru
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