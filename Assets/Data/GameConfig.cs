using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "WaterSort/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Bottle Animation")]
    public float selectHeight = 0.5f;     // Độ cao khi click nhấc bình
    public float pourHeight = 2.0f;       // Độ cao của bình rót so với bình nhận
    public float pourXOffset = 1.2f;      // Khoảng cách theo trục X khi rót
    public float pourAngle = 88f;         // Góc nghiêng lật chai

    [Header("Pouring Mechanics")]
    public float timePerUnit = 0.15f;     // Thời gian rót xong 1 vạch nước
    public float streamStartYOffset = -0.2f; // Tọa độ Y tia nước mọc ra (so với miệng bình)
    public float streamEndYOffset = -1.2f;   // Độ sâu tia nước cắm vào bình đích
    public float streamBaseWidth = 0.15f;    // Bề ngang của tia nước
}