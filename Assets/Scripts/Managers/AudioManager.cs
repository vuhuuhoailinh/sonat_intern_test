using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;      
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource pourSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;                           
    public AudioClip selectClip;
    public AudioClip deselectClip;
    public AudioClip bottleFullClip;
    public AudioClip corkClip;
    public AudioClip pourClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pourSource != null) pourSource.loop = true;

        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
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
    public void SetBGMState(bool isOn)
    {
        if (bgmSource != null) bgmSource.mute = !isOn;
    }

    public void SetSFXState(bool isOn)
    {
        if (sfxSource != null) sfxSource.mute = !isOn;
        if (pourSource != null) pourSource.mute = !isOn;
    }
}