using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;       // MỚI: Kênh phát nhạc nền
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource pourSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;                             // MỚI: File nhạc nền
    public AudioClip selectClip;
    public AudioClip deselectClip;
    public AudioClip bottleFullClip;
    public AudioClip corkClip;
    public AudioClip pourClip;

    void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ AudioManager không bị hủy khi đổi Scene (nếu sau này bạn có nhiều Scene)
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cài đặt cho Pour Source (Lặp lại)
        if (pourSource != null) pourSource.loop = true;

        // MỚI: Cài đặt và phát nhạc nền ngay khi vào game
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true; // Nhạc nền phải lặp lại liên tục
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StartPouring()
    {
        if (pourClip != null && pourSource != null && !pourSource.isPlaying)
        {
            pourSource.clip = pourClip;
            pourSource.Play();
        }
    }

    public void StopPouring()
    {
        if (pourSource != null && pourSource.isPlaying)
        {
            pourSource.Stop();
        }
    }
    // Tắt/Bật Nhạc nền
    public void SetBGMState(bool isOn)
    {
        if (bgmSource != null) bgmSource.mute = !isOn;
    }

    // Tắt/Bật Tiếng động (Nhấc chai, Rót nước, Win)
    public void SetSFXState(bool isOn)
    {
        if (sfxSource != null) sfxSource.mute = !isOn;
        if (pourSource != null) pourSource.mute = !isOn;
    }
}