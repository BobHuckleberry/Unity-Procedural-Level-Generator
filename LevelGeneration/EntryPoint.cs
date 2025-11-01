using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    private bool occupied = false;

    public bool IsOccupied()
    {
        return occupied;
    }

    public void SetOccupied(bool value = true)
    {
        occupied = value;
    }
}