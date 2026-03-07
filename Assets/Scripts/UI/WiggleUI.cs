using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class WiggleUI : MonoBehaviour
{
    [Header("Frames")]
    public Sprite frame1;
    public Sprite frame2;

    [Header("Settings")]
    public float interval = 0.2f;

    private Image img;
    private bool isFrame1 = true;

    void Awake()
    {
        img = GetComponent<Image>();

        if (frame1 == null) frame1 = img.sprite;
    }

    void OnEnable()
    {
        StartCoroutine(WiggleRoutine());
    }

    IEnumerator WiggleRoutine()
    {
        if (frame1 == null || frame2 == null) yield break;

        while (true)
        {
            yield return new WaitForSeconds(interval);

            isFrame1 = !isFrame1;

            img.sprite = isFrame1 ? frame1 : frame2;
        }
    }
}