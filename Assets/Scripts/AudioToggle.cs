using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AudioToggle : MonoBehaviour
{
    public enum AudioType { BGM, SFX }
    [Header("Loại Âm Thanh")]
    public AudioType targetAudio;

    [Header("Sprites")]
    public Sprite onSprite;  // Nét vẽ lúc BẬT
    public Sprite offSprite; // Nét vẽ lúc TẮT (Gạch chéo)

    private Image img;
    private bool isOn = true;

    void Awake()
    {
        img = GetComponent<Image>();
        UpdateVisuals();
    }

    // Kéo hàm này vào sự kiện OnClick() của Button
    public void OnToggleClicked()
    {
        isOn = !isOn; // Đảo trạng thái
        UpdateVisuals();

        // Gọi AudioManager tùy theo loại nút
        if (targetAudio == AudioType.BGM)
            AudioManager.Instance.SetBGMState(isOn);
        else
            AudioManager.Instance.SetSFXState(isOn);
    }

    private void UpdateVisuals()
    {
        if (img != null)
        {
            img.sprite = isOn ? onSprite : offSprite;
        }
    }
}