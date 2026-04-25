using System.Collections.Generic;
using UnityEngine;

public class PlayerOcclusionFader : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private UnityEngine.Camera renderCamera;
    [SerializeField] private LayerMask occluderMask;

    [Header("Fade")]
    [SerializeField] private float fadedAlpha = 0.35f;
    [SerializeField] private float fadeSpeed = 8f;

    [Header("Raycast Target")]
    [SerializeField] private Vector3 playerTargetOffset = new Vector3(0f, 1f, 0f);

    private readonly Dictionary<Renderer, MaterialPropertyBlock> blocks = new();
    private readonly Dictionary<Renderer, float> fadeValues = new();
    private readonly List<Renderer> cachedRenderers = new();

    private void Awake()
    {
        if (!renderCamera)
            renderCamera = UnityEngine.Camera.main;
    }

    private void LateUpdate()
    {
        if (!player || !renderCamera) return;

        cachedRenderers.Clear();
        cachedRenderers.AddRange(fadeValues.Keys);

        foreach (Renderer renderer in cachedRenderers)
        {
            if (!renderer)
            {
                fadeValues.Remove(renderer);
                blocks.Remove(renderer);
                continue;
            }

            fadeValues[renderer] = Mathf.MoveTowards(
                fadeValues[renderer],
                0f,
                fadeSpeed * Time.deltaTime
            );
        }

        Vector3 origin = renderCamera.transform.position;
        Vector3 target = player.position + playerTargetOffset;
        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            direction.normalized,
            distance,
            occluderMask,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponentInParent<Renderer>();
            if (!renderer) continue;

            if (!fadeValues.ContainsKey(renderer))
            {
                fadeValues.Add(renderer, 0f);
                blocks.Add(renderer, new MaterialPropertyBlock());
            }

            fadeValues[renderer] = Mathf.MoveTowards(
                fadeValues[renderer],
                1f,
                fadeSpeed * Time.deltaTime
            );
        }

        cachedRenderers.Clear();
        cachedRenderers.AddRange(fadeValues.Keys);

        foreach (Renderer renderer in cachedRenderers)
        {
            if (!renderer) continue;

            renderer.GetPropertyBlock(blocks[renderer]);
            blocks[renderer].SetFloat("_PlayerFade", fadeValues[renderer]);
            blocks[renderer].SetFloat("_FadedAlpha", fadedAlpha);
            renderer.SetPropertyBlock(blocks[renderer]);
        }
    }
}