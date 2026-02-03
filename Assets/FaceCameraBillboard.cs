using UnityEngine;

public class FaceCameraBillboard : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (!cam) return;

        // Make the bubble face the camera
        transform.rotation = Quaternion.LookRotation(
            transform.position - cam.transform.position,
            Vector3.up
        );
    }
}
