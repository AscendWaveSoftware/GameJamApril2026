using UnityEngine;

public class SunDiscFollower : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance = 500f;

    private void LateUpdate()
    {
        if (!sun || !cameraTransform) return;

        Vector3 sunDirection = -sun.transform.forward;
        transform.position = cameraTransform.position + sunDirection * distance;
        transform.forward = sunDirection;
    }
}
