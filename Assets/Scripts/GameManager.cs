using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Init, Playing, Animating, Win, Lose }
    public GameState currentState = GameState.Playing;

    [Header("System Configuration")]
    public GameConfig gameConfig;
    public LevelData[] allLevels; // MỚI: Mảng chứa tất cả các màn chơi
    private int currentLevelIndex = 0; // Theo dõi màn hiện tại

    [Header("Scene References")]
    [SerializeField] private Bottle[] allBottles;
    [SerializeField] private PourStream pourStream;

    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private WiggleUI levelTextWiggle;

    private Bottle selectedBottle;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    // MỚI: Hàm nạp dữ liệu Level vào các chai
    public void LoadLevel(int levelIndex)
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (allLevels == null || allLevels.Length == 0) return;

        if (levelIndex >= allLevels.Length) levelIndex = 0;

        LevelData dataToLoad = allLevels[levelIndex];

        // --- MỚI: CẬP NHẬT CHỮ LEVEL TRÊN UI ---
        if (levelTextWiggle != null && dataToLoad.levelNameFrame1 != null && dataToLoad.levelNameFrame2 != null)
        {
            // Tráo 2 frame mới vào kịch bản rung rinh
            levelTextWiggle.frame1 = dataToLoad.levelNameFrame1;
            levelTextWiggle.frame2 = dataToLoad.levelNameFrame2;

            // Cập nhật luôn hình ảnh hiện tại trên màn hình
            levelTextWiggle.GetComponent<UnityEngine.UI.Image>().sprite = dataToLoad.levelNameFrame1;
        }
        // ----------------------------------------

        currentState = GameState.Playing;
        selectedBottle = null;

        for (int i = 0; i < allBottles.Length; i++)
        {
            if (i < dataToLoad.bottles.Length)
                allBottles[i].Initialize(dataToLoad.bottleCapacity, dataToLoad.bottles[i].startColors);
            else
                allBottles[i].Initialize(dataToLoad.bottleCapacity, new Color[0]);
        }
    }

    #region Input Handling
    public void HandleClick(Bottle clickedBottle)
    {
        if (currentState != GameState.Playing) return;

        if (selectedBottle == null)
            ProcessFirstClick(clickedBottle);
        else
            ProcessSecondClick(clickedBottle);
    }

    private void ProcessFirstClick(Bottle clickedBottle)
    {
        if (!clickedBottle.IsEmpty())
        {
            selectedBottle = clickedBottle;
            selectedBottle.Motion.AnimateSelect();

            // SFX: Tiếng nhấc chai
            AudioManager.Instance.PlaySFX(AudioManager.Instance.selectClip);
        }
    }

    private void ProcessSecondClick(Bottle clickedBottle)
    {
        if (selectedBottle == clickedBottle)
        {
            DeselectCurrentBottle();
        }
        else if (CanPour(selectedBottle, clickedBottle))
        {
            currentState = GameState.Animating;

            selectedBottle.PourTo(
                target: clickedBottle,
                onStartPour: (start, end, color, duration) => {
                    pourStream.Show(start, end, color, duration);
                    // SFX: Bắt đầu tiếng nước chảy
                    AudioManager.Instance.StartPouring();
                },
                onEndPour: (duration) => {
                    pourStream.Hide(duration);
                    // SFX: Ngắt tiếng nước chảy
                    AudioManager.Instance.StopPouring();
                },
                onComplete: OnPourComplete
            );

            selectedBottle = null;
        }
        else
        {
            DeselectCurrentBottle();
        }
    }

    private void DeselectCurrentBottle()
    {
        selectedBottle.Motion.AnimateDeselect();
        selectedBottle = null;

        // SFX: Tiếng hạ chai
        AudioManager.Instance.PlaySFX(AudioManager.Instance.deselectClip);
    }
    #endregion

    #region Rules & Logic
    private bool CanPour(Bottle source, Bottle target)
    {
        if (target.IsFull()) return false;
        if (target.IsEmpty()) return true;
        return source.GetTopColor() == target.GetTopColor();
    }
    #endregion

    #region Game State & Flow
    private void OnPourComplete()
    {
        currentState = GameState.Playing;

        // 1. Kiểm tra xem đã THẮNG chưa
        bool isWin = true;
        foreach (Bottle bottle in allBottles)
        {
            if (!bottle.IsEmpty() && !bottle.IsCompleted)
            {
                isWin = false;
                break;
            }
        }

        if (isWin)
        {
            Debug.Log("🎉 You Win!");
            currentState = GameState.Win;
            if (winPanel != null) winPanel.SetActive(true);
            return; // Nếu thắng rồi thì thoát luôn, không check Lose nữa
        }

        // 2. Nếu chưa thắng, kiểm tra xem có bị BÍ ĐƯỜNG (Deadlock) không
        if (!HasAnyValidMove())
        {
            Debug.Log("💀 Out of moves!");
            currentState = GameState.Lose;
            if (losePanel != null) losePanel.SetActive(true);
        }
    }

    // THUẬT TOÁN QUÉT BƯỚC ĐI
    private bool HasAnyValidMove()
    {
        for (int i = 0; i < allBottles.Length; i++)
        {
            Bottle source = allBottles[i];

            // Nếu bình rỗng hoặc đã đầy 1 màu (hoàn thành) thì không lấy làm nguồn rót
            if (source.IsEmpty() || source.IsCompleted) continue;

            for (int j = 0; j < allBottles.Length; j++)
            {
                if (i == j) continue; // Không tự rót vào chính mình

                Bottle target = allBottles[j];
                if (target.IsFull()) continue; // Bình đích đã đầy thì bỏ qua

                // Nếu có 1 bình đích rỗng -> LUÔN CÓ ĐƯỜNG ĐI
                if (target.IsEmpty()) return true;

                // Nếu bình đích chưa đầy và CÓ CÙNG MÀU ở trên cùng -> CÓ ĐƯỜNG ĐI
                if (source.GetTopColor() == target.GetTopColor()) return true;
            }
        }

        // Duyệt hết mọi khả năng mà không ra được cặp nào hợp lệ -> Bí đường (Deadlock)
        return false;
    }

    public void RestartLevel()
    {
        if (losePanel != null) losePanel.SetActive(false); // Ẩn bảng Lose đi nếu đang hiện
        LoadLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        LoadLevel(currentLevelIndex);
    }
    #endregion
}