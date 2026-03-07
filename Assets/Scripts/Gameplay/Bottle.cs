using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BottleLiquid))]
[RequireComponent(typeof(BottleMotion))]
public class Bottle : MonoBehaviour
{
    private int capacity;
    private Stack<Color> waterStack = new Stack<Color>();

    public bool IsCompleted { get; private set; }
    public BottleLiquid Liquid { get; private set; }
    public BottleMotion Motion { get; private set; }

    void Awake()
    {
        Liquid = GetComponent<BottleLiquid>();
        Motion = GetComponent<BottleMotion>();
    }
    public void Initialize(int cap, Color[] startColors)
    {
        capacity = cap;
        waterStack.Clear();
        IsCompleted = false;

        Liquid.Initialize();
        Motion.Initialize();

        foreach (var c in startColors)
            PushWater(c, 0f);
    }

    void OnMouseDown()
    {
        if (IsCompleted) return;
        GameManager.Instance.HandleClick(this);
    }

    #region Water Logic
    public void PushWater(Color c, float duration = -1f)
    {
        if (duration < 0f) duration = GameManager.Instance.gameConfig.timePerUnit;

        Color colorBelow = IsEmpty() ? Color.clear : GetTopColor();
        int index = waterStack.Count;

        waterStack.Push(c);
        Liquid.FillWater(index, c, waterStack.Count, capacity, duration, colorBelow);
    }

    public Color PopWater(float duration = -1f)
    {
        if (duration < 0f) duration = GameManager.Instance.gameConfig.timePerUnit;

        Color c = waterStack.Pop();
        int index = waterStack.Count;

        Liquid.WithdrawWater(index, waterStack.Count, capacity, duration, GetTopColor(), IsEmpty());
        return c;
    }

    public bool IsFull() => waterStack.Count >= capacity;
    public bool IsEmpty() => waterStack.Count == 0;
    public Color GetTopColor() => IsEmpty() ? Color.clear : waterStack.Peek();
    public int GetAvailableSpace() => capacity - waterStack.Count;

    public int GetPourableUnits()
    {
        if (IsEmpty()) return 0;
        Color topColor = GetTopColor();
        int count = 0;
        foreach (Color c in waterStack)
        {
            if (c == topColor) count++;
            else break;
        }
        return count;
    }

    public void CheckCompletion()
    {
        if (waterStack.Count < capacity) return;

        Color topColor = GetTopColor();
        foreach (Color c in waterStack)
        {
            if (c != topColor) return;
        }

        IsCompleted = true;
        Motion.PlayWinAnimation();

        // MỚI THÊM: SFX khi chai đầy và nắp đóng
        AudioManager.Instance.PlaySFX(AudioManager.Instance.bottleFullClip);
        // Có thể dùng DOTween/Invoke để delay tiếng đóng nắp cho khớp animation
        Invoke(nameof(PlayCorkSound), 0.3f);
    }

    private void PlayCorkSound()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.corkClip);
    }
    #endregion

    #region Pour Animation Orchestration
    public void PourTo(Bottle target, System.Action<Vector3, Vector3, Color, float> onStartPour, System.Action<float> onEndPour, System.Action onComplete)
    {
        int unitsToPour = Mathf.Min(this.GetPourableUnits(), target.GetAvailableSpace());
        GameConfig config = GameManager.Instance.gameConfig; // Lấy config
        Color pourColor = GetTopColor();

        Motion.AnimatePour(target.transform, unitsToPour, config,
            onReadyToPour: (spoutPos) => {
                Vector3 streamStart = spoutPos + new Vector3(0f, config.streamStartYOffset, 0f);
                Vector3 streamEnd = new Vector3(streamStart.x, target.transform.position.y + config.streamEndYOffset, 0f);

                onStartPour?.Invoke(streamStart, streamEnd, pourColor, config.timePerUnit);
            },
            onPourUnit: () => {
                Color water = this.PopWater(config.timePerUnit);
                target.PushWater(water, config.timePerUnit);
            },
            onFinishPourLoop: () => {
                onEndPour?.Invoke(config.timePerUnit);
            },
            onComplete: () => {
                target.CheckCompletion();
                onComplete?.Invoke();
            }
        );
    }
    #endregion
}