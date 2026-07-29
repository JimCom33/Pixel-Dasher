using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField, Min(0.01f)] private float smoothTime = 0.2f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 destination = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            destination,
            ref velocity,
            smoothTime);
    }

    private void OnValidate()
    {
        smoothTime = Mathf.Max(0.01f, smoothTime);
    }
}
