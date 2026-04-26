using UnityEngine;

namespace Player
{
    public sealed class ScreenTitleManager : MonoBehaviour
    {
        private const string RuntimeObjectName = "__ScreenTitleManager";
        private const float FadeOutSeconds = 0.8f;

        private static ScreenTitleManager _instance;

        private string _currentText = string.Empty;
        private float _showUntilUnscaledTime;
        private float _fadeStartUnscaledTime;
        private bool _isVisible;
        private bool _waitForKeyDismiss;
        private KeyCode _dismissKey = KeyCode.None;

        private GUIStyle _titleStyle;
        private Texture2D _whiteTex;

        public static void ShowTitle(string text, float durationSeconds)
        {
            EnsureInstance();
            _instance.ShowInternal(text, durationSeconds);
        }

        public static void ShowTitleUntilKey(string text, KeyCode dismissKey)
        {
            EnsureInstance();
            _instance.ShowUntilKeyInternal(text, dismissKey);
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                return;
            }

            _instance = FindFirstObjectByType<ScreenTitleManager>();
            if (_instance != null)
            {
                return;
            }

            GameObject go = new GameObject(RuntimeObjectName);
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<ScreenTitleManager>();
        }

        private void ShowInternal(string text, float durationSeconds)
        {
            _currentText = string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim();

            float hold = Mathf.Max(0f, durationSeconds);
            float now = Time.unscaledTime;

            _showUntilUnscaledTime = now + hold;
            _fadeStartUnscaledTime = _showUntilUnscaledTime;
            _waitForKeyDismiss = false;
            _dismissKey = KeyCode.None;
            _isVisible = !string.IsNullOrEmpty(_currentText);
        }

        private void ShowUntilKeyInternal(string text, KeyCode dismissKey)
        {
            _currentText = string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim();

            _waitForKeyDismiss = dismissKey != KeyCode.None;
            _dismissKey = dismissKey;

            float now = Time.unscaledTime;
            _showUntilUnscaledTime = float.PositiveInfinity;
            _fadeStartUnscaledTime = float.PositiveInfinity;

            if (!_waitForKeyDismiss)
            {
                _showUntilUnscaledTime = now;
                _fadeStartUnscaledTime = now;
            }

            _isVisible = !string.IsNullOrEmpty(_currentText);
        }

        private void OnGUI()
        {
            if (!_isVisible)
            {
                return;
            }

            if (string.IsNullOrEmpty(_currentText))
            {
                _isVisible = false;
                return;
            }

            float now = Time.unscaledTime;
            float alpha = 1f;

            if (_waitForKeyDismiss)
            {
                if (_dismissKey != KeyCode.None && Event.current.type == EventType.KeyDown && Event.current.keyCode == _dismissKey)
                {
                    _waitForKeyDismiss = false;
                    _showUntilUnscaledTime = now;
                    _fadeStartUnscaledTime = now;
                }
            }

            if (!_waitForKeyDismiss && now > _fadeStartUnscaledTime)
            {
                float fadeT = Mathf.Clamp01((now - _fadeStartUnscaledTime) / FadeOutSeconds);
                alpha = 1f - fadeT;
                if (alpha <= 0.001f)
                {
                    _isVisible = false;
                    return;
                }
            }

            BuildStyleIfNeeded();

            float maxWidth = Mathf.Max(320f, Screen.width * 0.72f);
            float minHeight = Mathf.Max(48f, Screen.height * 0.08f);
            float textHeight = _titleStyle.CalcHeight(new GUIContent(_currentText), maxWidth);
            float panelHeight = Mathf.Max(minHeight, textHeight + 26f);

            float x = (Screen.width - maxWidth) * 0.5f;
            float y = Screen.height * 0.2f;

            Rect panelRect = new Rect(x, y, maxWidth, panelHeight);
            Rect textRect = new Rect(panelRect.x + 16f, panelRect.y + 13f, panelRect.width - 32f, panelRect.height - 20f);

            Color previousColor = GUI.color;

            GUI.color = new Color(0f, 0f, 0f, 0.55f * alpha);
            GUI.DrawTexture(panelRect, _whiteTex);

            GUI.color = new Color(1f, 0.94f, 0.8f, alpha);
            GUI.Label(textRect, _currentText, _titleStyle);

            GUI.color = previousColor;
        }

        private void BuildStyleIfNeeded()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    fontSize = Mathf.RoundToInt(Mathf.Max(18f, Screen.height * 0.035f)),
                    fontStyle = FontStyle.Bold
                };

                _titleStyle.normal.textColor = Color.white;
                _titleStyle.richText = true;
            }

            if (_whiteTex == null)
            {
                _whiteTex = new Texture2D(1, 1);
                _whiteTex.SetPixel(0, 0, Color.white);
                _whiteTex.Apply();
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }

            if (_whiteTex != null)
            {
                Destroy(_whiteTex);
            }
        }
    }
}
