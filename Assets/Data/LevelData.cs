using UnityEngine;

[CreateAssetMenu(fileName = "Level_01", menuName = "WaterSort/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public int bottleCapacity = 4;

    [Header("UI Level Text (Wiggle Frames)")]
    public Sprite levelNameFrame1;
    public Sprite levelNameFrame2;

    [Header("Bottles Setup")]
    public BottleSetup[] bottles;
}

[System.Serializable]
public class BottleSetup
{
    public Color[] startColors;
}