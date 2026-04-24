using UnityEngine;

public class CameraFollowDummy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -8f);
    [SerializeField] private bool useSmoothFollow = true;
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 currentVelocity;

    private void Awake()
    {
        if (target == null)
        {
            PlayerMovement dummy = FindFirstObjectByType<PlayerMovement>();
            if (dummy != null)
            {
                target = dummy.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        if (useSmoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.LookAt(target);
    }
}

