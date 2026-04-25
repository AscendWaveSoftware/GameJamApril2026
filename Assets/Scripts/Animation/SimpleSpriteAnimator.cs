using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleSpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float framesPerSecond = 8f;

    private int _frameIndex;
    private float _timer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (frames != null && frames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = frames[0];
        }
    }

    private void Update()
    {
        if (frames == null || frames.Length <= 1 || spriteRenderer == null)
        {
            return;
        }

        float frameDuration = 1f / Mathf.Max(0.01f, framesPerSecond);
        _timer += Time.deltaTime;

        while (_timer >= frameDuration)
        {
            _timer -= frameDuration;
            _frameIndex = (_frameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[_frameIndex];
        }
    }
}

