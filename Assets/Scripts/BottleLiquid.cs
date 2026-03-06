using UnityEngine;
using DG.Tweening;

public class BottleLiquid : MonoBehaviour
{
    [Header("Liquid Elements")]
    [SerializeField] private SpriteRenderer[] waterVisuals;

    [Header("3D Surface")]
    [SerializeField] private SpriteRenderer topSurfaceRenderer;
    [SerializeField] private SpriteRenderer[] separatorVisuals;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;

    private Vector3[] originalWaterScales;

    public void Initialize()
    {
        InitWaterVisuals();
        InitSeparators();
    }

    #region Initialization
    private void InitWaterVisuals()
    {
        // 1. CHỈ lưu kích thước gốc nếu mảng này chưa từng được tạo
        if (originalWaterScales == null || originalWaterScales.Length == 0)
        {
            originalWaterScales = new Vector3[waterVisuals.Length];
            for (int i = 0; i < waterVisuals.Length; i++)
            {
                originalWaterScales[i] = waterVisuals[i].transform.localScale;
            }
        }

        // 2. Tắt các khối nước đi và DẬP TẮT mọi animation đang chạy dở
        for (int i = 0; i < waterVisuals.Length; i++)
        {
            waterVisuals[i].transform.DOKill(); // Ngắt DOTween an toàn
            waterVisuals[i].gameObject.SetActive(false);
        }

        if (topSurfaceRenderer != null)
        {
            topSurfaceRenderer.transform.DOKill();
            topSurfaceRenderer.gameObject.SetActive(false);
        }
    }

    private void InitSeparators()
    {
        if (separatorVisuals == null || separatorVisuals.Length != waterVisuals.Length - 1) return;

        for (int i = 0; i < separatorVisuals.Length; i++)
        {
            if (separatorVisuals[i] == null) continue;

            float yPos = GetSurfaceTargetY(i + 1, waterVisuals.Length);
            SetLocalY(separatorVisuals[i].transform, yPos);
            separatorVisuals[i].gameObject.SetActive(false);
        }
    }
    #endregion

    #region Water Filling
    public void FillWater(int index, Color color, int currentCount, int capacity, float duration, Color colorBelow)
    {
        float targetY = GetSurfaceTargetY(currentCount, capacity);
        float startY = GetSurfaceTargetY(currentCount - 1, capacity);

        AnimateMainWaterFill(index, color, duration);
        UpdateSeparatorOnFill(index, color, colorBelow);
        AnimateTopSurfaceFill(color, startY, targetY, duration);
    }

    private void AnimateMainWaterFill(int index, Color color, float duration)
    {
        waterVisuals[index].gameObject.SetActive(true);
        waterVisuals[index].color = color;
        Vector3 targetScale = originalWaterScales[index];

        if (duration > 0)
        {
            waterVisuals[index].transform.localScale = new Vector3(targetScale.x, 0, targetScale.z);
            waterVisuals[index].transform.DOScaleY(targetScale.y, duration).SetEase(Ease.Linear);
        }
        else
        {
            waterVisuals[index].transform.localScale = targetScale;
        }
    }

    private void UpdateSeparatorOnFill(int index, Color color, Color colorBelow)
    {
        if (index <= 0 || separatorVisuals == null || index - 1 >= separatorVisuals.Length || separatorVisuals[index - 1] == null) return;

        bool isDifferentColor = (color != colorBelow);
        separatorVisuals[index - 1].gameObject.SetActive(isDifferentColor);

        if (isDifferentColor)
            separatorVisuals[index - 1].color = color;
    }

    private void AnimateTopSurfaceFill(Color color, float startY, float targetY, float duration)
    {
        if (topSurfaceRenderer == null) return;

        topSurfaceRenderer.gameObject.SetActive(true);
        topSurfaceRenderer.color = color;

        if (duration > 0)
        {
            SetLocalY(topSurfaceRenderer.transform, startY);
            topSurfaceRenderer.transform.DOLocalMoveY(targetY, duration).SetEase(Ease.Linear);
        }
        else
        {
            SetLocalY(topSurfaceRenderer.transform, targetY);
        }
    }
    #endregion

    #region Water Withdrawing
    public void WithdrawWater(int index, int currentCount, int capacity, float duration, Color nextTopColor, bool isEmpty)
    {
        float targetY = GetSurfaceTargetY(currentCount, capacity);

        AnimateMainWaterWithdraw(index, duration);
        AnimateTopSurfaceWithdraw(targetY, duration, nextTopColor, isEmpty);
    }

    private void AnimateMainWaterWithdraw(int index, float duration)
    {
        if (duration > 0)
        {
            waterVisuals[index].transform.DOScaleY(0f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => DisableWaterLayer(index));
        }
        else
        {
            DisableWaterLayer(index);
        }
    }

    private void DisableWaterLayer(int index)
    {
        waterVisuals[index].gameObject.SetActive(false);
        if (index > 0 && separatorVisuals != null && index - 1 < separatorVisuals.Length && separatorVisuals[index - 1] != null)
        {
            separatorVisuals[index - 1].gameObject.SetActive(false);
        }
    }

    private void AnimateTopSurfaceWithdraw(float targetY, float duration, Color nextTopColor, bool isEmpty)
    {
        if (topSurfaceRenderer == null) return;

        if (duration > 0)
        {
            topSurfaceRenderer.transform.DOLocalMoveY(targetY, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => UpdateSurfaceStateAfterWithdraw(nextTopColor, isEmpty));
        }
        else
        {
            SetLocalY(topSurfaceRenderer.transform, targetY);
            UpdateSurfaceStateAfterWithdraw(nextTopColor, isEmpty);
        }
    }

    private void UpdateSurfaceStateAfterWithdraw(Color nextTopColor, bool isEmpty)
    {
        if (isEmpty) topSurfaceRenderer.gameObject.SetActive(false);
        else topSurfaceRenderer.color = nextTopColor;
    }
    #endregion

    #region Math Helpers
    private float GetSurfaceTargetY(int currentCount, int capacity)
    {
        return Mathf.Lerp(bottomPoint.localPosition.y, topPoint.localPosition.y, (float)currentCount / capacity);
    }

    private void SetLocalY(Transform objTransform, float yValue)
    {
        objTransform.localPosition = new Vector3(objTransform.localPosition.x, yValue, objTransform.localPosition.z);
    }
    #endregion
}