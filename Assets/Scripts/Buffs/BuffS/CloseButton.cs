using UnityEngine;

public class CloseButton : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject uicamera;
    [SerializeField] private GameObject canvas;
    public void OnClick()
    {
        canvas = transform.gameObject;
        canvas.SetActive(false);
        player.SetActive(true);
        camera.SetActive(true);
        uicamera.SetActive(false);
    }
}
