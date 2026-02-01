using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera arCamera;

    void LateUpdate()
    {
        if (!arCamera) return;

        Vector3 direction = arCamera.transform.position - transform.position;
        direction.y = 0f; // keep character upright

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
