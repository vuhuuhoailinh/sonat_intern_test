using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject menuOverlay; // Cục nền đen chứa toàn bộ cái Menu Board

    void Start()
    {
        if (menuOverlay != null) menuOverlay.SetActive(false);
    }

    public void OpenMenu()
    {
        if (menuOverlay != null) menuOverlay.SetActive(true);
        Time.timeScale = 0f; // Dừng game
    }

    public void CloseMenu()
    {
        if (menuOverlay != null) menuOverlay.SetActive(false);
        Time.timeScale = 1f; // Chạy lại game
    }
}