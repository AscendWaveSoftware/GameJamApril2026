using UnityEngine;

namespace Camera
{
    public class PlayerCameraHandler : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Follow")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -8f);
        [SerializeField] private bool useSmoothFollow = true;
        [SerializeField] private float smoothTime = 0.15f;

        [Header("View")]
        [SerializeField] private float lookAtVerticalOffset = 1.5f;

        private Vector3 currentVelocity;

        private void Awake()
        {
            if (target == null)
            {
                TryFindTarget();
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            // prevent when not on camera
            if (target == transform)
            {
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            if (useSmoothFollow)
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
            }
            else
            {
                transform.position = desiredPosition;
            }

            Vector3 lookPoint = target.position + Vector3.up * lookAtVerticalOffset;
            transform.LookAt(lookPoint);
        }

        private void TryFindTarget()
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null && player.transform != transform)
            {
                target = player.transform;
                return;
            }

            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null && taggedPlayer.transform != transform)
            {
                target = taggedPlayer.transform;
            }
        }
    }
}