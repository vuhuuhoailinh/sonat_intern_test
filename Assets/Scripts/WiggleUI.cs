using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))] // Bắt buộc vật thể phải có Image
public class WiggleUI : MonoBehaviour
{
    [Header("Frames")]
    public Sprite frame1;
    public Sprite frame2;

    [Header("Settings")]
    public float interval = 0.2f; // Tốc độ đổi frame (giây). Càng nhỏ càng giật nhanh.

    private Image img;
    private bool isFrame1 = true;

    void Awake()
    {
        img = GetComponent<Image>();

        // Nếu chưa gán frame1, tự động lấy ảnh gốc làm frame 1
        if (frame1 == null) frame1 = img.sprite;
    }

    void OnEnable()
    {
        // Mỗi khi nút/ảnh này được bật lên, bắt đầu hiệu ứng giật
        StartCoroutine(WiggleRoutine());
    }

    IEnumerator WiggleRoutine()
    {
        // Nếu thiếu ảnh thì không làm gì cả
        if (frame1 == null || frame2 == null) yield break;

        while (true)
        {
            // Đợi một khoảng thời gian
            yield return new WaitForSeconds(interval);

            // Tráo đổi cờ
            isFrame1 = !isFrame1;

            // Lắp ảnh mới vào
            img.sprite = isFrame1 ? frame1 : frame2;
        }
    }
}