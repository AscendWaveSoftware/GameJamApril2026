using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyHealthBarHandler : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.25f, 0f);
        [SerializeField] private float width = 1.2f;
        [SerializeField] private float height = 0.14f;
        [SerializeField] private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color fillColor = new Color(0.15f, 0.85f, 0.15f, 1f);

        private EnemyBase _enemyBase;
        private Transform _barRoot;
        private SpriteRenderer _backgroundRenderer;
        private SpriteRenderer _fillRenderer;
        private UnityEngine.Camera _mainCamera;

        private static Sprite _barSprite;

        private void Awake()
        {
            _enemyBase = GetComponent<EnemyBase>();
            _mainCamera = UnityEngine.Camera.main;

            CreateBarObjects();
            UpdateBar();
        }

        private void OnEnable()
        {
            _enemyBase.OnDamage += UpdateBar;
            _enemyBase.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            _enemyBase.OnDamage -= UpdateBar;
            _enemyBase.OnDeath -= HandleDeath;
        }

        private void LateUpdate()
        {
            if (_barRoot == null)
            {
                return;
            }

            _barRoot.position = transform.position + offset;

            if (_mainCamera == null)
            {
                _mainCamera = UnityEngine.Camera.main;
            }

            if (_mainCamera != null)
            {
                _barRoot.forward = _mainCamera.transform.forward;
            }
        }

        private void HandleDeath()
        {
            if (_barRoot != null)
            {
                Destroy(_barRoot.gameObject);
            }
        }

        private void UpdateBar()
        {
            if (_backgroundRenderer == null || _fillRenderer == null)
            {
                return;
            }

            float maxHealth = Mathf.Max(0.0001f, _enemyBase.MaxHealth);
            float ratio = Mathf.Clamp01(_enemyBase.CurrentHealth / maxHealth);

            _backgroundRenderer.transform.localScale = new Vector3(width, height, 1f);
            _fillRenderer.transform.localScale = new Vector3(width * ratio, height, 1f);
            _fillRenderer.transform.localPosition = new Vector3(-((width - (width * ratio)) * 0.5f), 0f, -0.01f);
        }

        private void CreateBarObjects()
        {
            _barRoot = new GameObject("HealthBar").transform;
            _barRoot.SetParent(transform, false);
            _barRoot.localPosition = offset;

            var background = new GameObject("Background");
            background.transform.SetParent(_barRoot, false);
            _backgroundRenderer = background.AddComponent<SpriteRenderer>();
            _backgroundRenderer.sprite = GetBarSprite();
            _backgroundRenderer.color = backgroundColor;

            var fill = new GameObject("Fill");
            fill.transform.SetParent(_barRoot, false);
            _fillRenderer = fill.AddComponent<SpriteRenderer>();
            _fillRenderer.sprite = GetBarSprite();
            _fillRenderer.color = fillColor;

            int baseSortingOrder = _enemyBase.SpriteRenderer != null ? _enemyBase.SpriteRenderer.sortingOrder : 0;
            _backgroundRenderer.sortingOrder = baseSortingOrder + 10;
            _fillRenderer.sortingOrder = baseSortingOrder + 11;
        }

        private static Sprite GetBarSprite()
        {
            if (_barSprite != null)
            {
                return _barSprite;
            }

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            _barSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            return _barSprite;
        }
    }
}

