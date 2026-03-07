using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AudioToggle : MonoBehaviour
{
    public enum AudioType { BGM, SFX }
    [Header("Loại Âm Thanh")]
    public AudioType targetAudio;

    [Header("Sprites")]
    public Sprite onSprite; 
    public Sprite offSprite; 

    private Image img;
    private bool isOn = true;

    void Awake()
    {
        img = GetComponent<Image>();
        UpdateVisuals();
    }

    public void OnToggleClicked()
    {
        isOn = !isOn;
        UpdateVisuals();

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