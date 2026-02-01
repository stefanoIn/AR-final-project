using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;

        if (arCamera == null)
        {
            Debug.LogWarning("[AR-MUSEUM][FaceCamera] Camera.main NOT found");
        }
        else
        {
            Debug.Log("[AR-MUSEUM][FaceCamera] Camera.main assigned");
        }
    }

    void LateUpdate()
    {
        if (!arCamera)
            return;

        Vector3 direction = arCamera.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
