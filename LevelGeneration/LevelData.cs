using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    private int count = 0;
    public int LevelCount
    {
        get => count;
        set => count = value;
    }
    private void OnEnable()
    {
        count = 0;
    }
}
