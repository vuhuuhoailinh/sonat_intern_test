using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject menuOverlay;

    void Start()
    {
        if (menuOverlay != null) menuOverlay.SetActive(false);
    }

    public void OpenMenu()
    {
        if (menuOverlay != null) menuOverlay.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        if (menuOverlay != null) menuOverlay.SetActive(false);
        Time.timeScale = 1f;
    }
}