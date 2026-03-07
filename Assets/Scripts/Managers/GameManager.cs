using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Init, Playing, Animating, Win, Lose }
    public GameState currentState = GameState.Playing;

    [Header("System Configuration")]
    public GameConfig gameConfig;
    public LevelData[] allLevels;
    private int currentLevelIndex = 0;

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


    public void LoadLevel(int levelIndex)
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (allLevels == null || allLevels.Length == 0) return;

        if (levelIndex >= allLevels.Length) levelIndex = 0;

        LevelData dataToLoad = allLevels[levelIndex];

        if (levelTextWiggle != null && dataToLoad.levelNameFrame1 != null && dataToLoad.levelNameFrame2 != null)
        {
            levelTextWiggle.frame1 = dataToLoad.levelNameFrame1;
            levelTextWiggle.frame2 = dataToLoad.levelNameFrame2;
            levelTextWiggle.GetComponent<UnityEngine.UI.Image>().sprite = dataToLoad.levelNameFrame1;
        }

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
                    AudioManager.Instance.StartPouring();
                },
                onEndPour: (duration) => {
                    pourStream.Hide(duration);
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
            Debug.Log("You Win!");
            currentState = GameState.Win;
            if (winPanel != null) winPanel.SetActive(true);
            return;
        }

        if (!HasAnyValidMove())
        {
            Debug.Log("Out of moves!");
            currentState = GameState.Lose;
            if (losePanel != null) losePanel.SetActive(true);
        }
    }

    private bool HasAnyValidMove()
    {
        for (int i = 0; i < allBottles.Length; i++)
        {
            Bottle source = allBottles[i];

            if (source.IsEmpty() || source.IsCompleted) continue;

            for (int j = 0; j < allBottles.Length; j++)
            {
                if (i == j) continue;

                Bottle target = allBottles[j];
                if (target.IsFull()) continue;

                if (target.IsEmpty()) return true;

                if (source.GetTopColor() == target.GetTopColor()) return true;
            }
        }

        return false;
    }

    public void RestartLevel()
    {
        if (losePanel != null) losePanel.SetActive(false);
        LoadLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        LoadLevel(currentLevelIndex);
    }
    #endregion
}