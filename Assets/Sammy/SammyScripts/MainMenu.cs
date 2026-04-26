using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject creditsPanel;

    private const string MAIN_LEVEL_STRING = "SammyScene";
    private bool isActive = false;

    private void Awake()
    {
        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(MAIN_LEVEL_STRING);
        });

        creditsButton.onClick.AddListener(() =>
        {
            isActive = !isActive;

           creditsPanel.SetActive(isActive);

        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        creditsPanel.SetActive(false);
    }
}
