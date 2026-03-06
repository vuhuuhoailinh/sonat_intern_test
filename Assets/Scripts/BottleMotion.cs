using UnityEngine;
using DG.Tweening;

public class BottleMotion : MonoBehaviour
{
    [Header("Motion Elements")]
    [SerializeField] private GameObject corkObject;

    [HideInInspector] public Vector3 originalPos;
    private Vector3 corkOriginalLocalPos;
    private bool isPosInitialized = false;
    public void Initialize()
    {
        // 1. CHỈ lưu vị trí gốc ở lần khởi tạo ĐẦU TIÊN
        if (!isPosInitialized)
        {
            originalPos = transform.position;
            if (corkObject != null)
            {
                corkOriginalLocalPos = corkObject.transform.localPosition;
            }
            isPosInitialized = true;
        }

        // 2. Dập tắt mọi Animation (nhấc, lật, rung) đang chạy dở trên vỏ chai
        transform.DOKill();

        // 3. Ép vỏ chai cắm thẳng xuống đất về lại vị trí và góc quay ban đầu
        transform.position = originalPos;
        transform.rotation = Quaternion.identity;

        // 4. Dọn dẹp nắp bần (nắp bị văng ra lúc Win màn trước)
        if (corkObject != null)
        {
            corkObject.transform.DOKill();
            corkObject.transform.localPosition = corkOriginalLocalPos;
            corkObject.SetActive(false);
        }
    }

    public void PlayWinAnimation()
    {
        // ... (Giữ nguyên như cũ)
        if (corkObject != null)
        {
            corkObject.SetActive(true);
            corkObject.transform.localPosition = corkOriginalLocalPos + Vector3.up * 1.5f;
            corkObject.transform.DOLocalMove(corkOriginalLocalPos, 0.5f).SetEase(Ease.OutBounce);
        }
        transform.DOPunchScale(new Vector3(0.15f, -0.1f, 0f), 0.5f, 5, 0.5f);
        transform.DOPunchPosition(Vector3.up * 0.2f, 0.5f, 2, 0.5f);
    }

    public void AnimateSelect() => transform.DOMoveY(originalPos.y + GameManager.Instance.gameConfig.selectHeight, 0.2f).SetEase(Ease.OutQuad);
    public void AnimateDeselect() => transform.DOMoveY(originalPos.y, 0.2f).SetEase(Ease.OutQuad);

    // Thay đổi tham số truyền vào
    public void AnimatePour(Transform targetTransform, int unitsToPour, GameConfig config,
            System.Action<Vector3> onReadyToPour,
            System.Action onPourUnit,
            System.Action onFinishPourLoop,
            System.Action onComplete)
    {
        float currentXOffset = transform.position.x < targetTransform.position.x ? -config.pourXOffset : config.pourXOffset;
        Vector3 targetPos = targetTransform.position + new Vector3(currentXOffset, config.pourHeight, 0);
        float rotationAngle = transform.position.x < targetTransform.position.x ? -config.pourAngle : config.pourAngle;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(targetPos, 0.5f).SetEase(Ease.InOutSine));
        seq.Append(transform.DORotate(new Vector3(0, 0, rotationAngle), 0.2f));

        seq.AppendCallback(() => {
            Vector3 spoutPos = transform.position + transform.up;
            spoutPos.x = targetTransform.position.x;
            onReadyToPour?.Invoke(spoutPos);
        });

        for (int i = 0; i < unitsToPour; i++)
        {
            seq.AppendCallback(() => onPourUnit?.Invoke());
            seq.AppendInterval(config.timePerUnit);
        }

        seq.AppendCallback(() => onFinishPourLoop?.Invoke());

        seq.AppendInterval(0.2f);
        seq.Append(transform.DORotate(Vector3.zero, 0.2f));
        seq.Append(transform.DOMove(originalPos, 0.5f).SetEase(Ease.InOutSine));

        seq.OnComplete(() => onComplete?.Invoke());
    }
}