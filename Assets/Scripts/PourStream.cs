using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class PourStream : MonoBehaviour
{
    private SpriteRenderer streamRenderer;
    private Vector3 targetPosCached;
    private Sequence currentTween;

    void Awake()
    {
        streamRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void Show(Vector3 startPos, Vector3 endPos, Color color, float duration)
    {
        gameObject.SetActive(true);
        streamRenderer.color = color;
        targetPosCached = new Vector3(startPos.x, endPos.y, startPos.z);

        transform.position = startPos;
        transform.rotation = Quaternion.identity;

        float distance = Mathf.Abs(startPos.y - targetPosCached.y);
        currentTween?.Kill();

        // MỚI: Đọc baseWidth từ GameConfig
        float width = GameManager.Instance.gameConfig.streamBaseWidth;
        transform.localScale = new Vector3(width, 0f, 1f);

        currentTween = DOTween.Sequence();
        currentTween.Append(transform.DOScaleY(distance, duration * 0.5f).SetEase(Ease.OutQuad));
    }

    public void Hide(float duration)
    {
        // ... (Giữ nguyên như cũ)
        currentTween?.Kill();
        currentTween = DOTween.Sequence();
        currentTween.Append(transform.DOScaleX(0f, duration * 0.5f));
        currentTween.Join(transform.DOScaleY(0f, duration * 0.5f).SetEase(Ease.InQuad));
        currentTween.Join(transform.DOMove(targetPosCached, duration * 0.5f).SetEase(Ease.InQuad));
        currentTween.OnComplete(() => gameObject.SetActive(false));
    }
}