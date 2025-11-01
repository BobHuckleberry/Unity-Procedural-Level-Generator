using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DungeonPart : MonoBehaviour
{
    [SerializeField]
    public List<Transform> entryPoints; // Changed to public

    [SerializeField]
    private BoxCollider boxCollider;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private GameObject fillerWall;

    // DungeonPart Constructor
    public DungeonPart()
    {
        entryPoints = new List<Transform>();
        boxCollider = new BoxCollider();
        layerMask = new LayerMask();
    }
    private HashSet<Transform> usedEntryPoints = new HashSet<Transform>();

    // Returns a list of all unused entry points
    public List<Transform> GetAvailableEntryPoints()
    {
        return entryPoints.Where(ep => !usedEntryPoints.Contains(ep)).ToList();
    }

    // Marks an entry point as used
    public void UseEntryPoint(Transform entryPoint)
    {
        usedEntryPoints.Add(entryPoint);
    }

    public bool HasAvailableEntryPoints(out Transform entryPoint)
    {
        entryPoint = null;
        foreach (Transform entry in entryPoints)
        {
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint entryComponent))
            {
                Debug.Log($"Entry point {entry.name} occupied: {entryComponent.IsOccupied()}");
                if (!entryComponent.IsOccupied())
                {
                    entryPoint = entry;
                    entryComponent.SetOccupied();
                    return true;
                }
            }
            else
            {
                Debug.LogWarning($"Entry point {entry.name} is missing EntryPoint component.");
            }
        }
        return false;
    }

    public void UnuseEntrypoint(Transform entryPoint)
    {
        if (entryPoint.TryGetComponent<EntryPoint>(out EntryPoint entry))
        {
            entry.SetOccupied(false);
        }
    }

    public void FillEmptyDoors()
    {
        GetAvailableEntryPoints().ForEach(entry =>
        {
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint ep))
            {
                ep.SetOccupied();
                Instantiate(fillerWall, entry.position, entry.rotation, transform);
            }
        });
    }

    public void ResetEntryPoints()
    {
        foreach (Transform entry in entryPoints)
        {
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint ep))
            {
                ep.SetOccupied(false); // Free all entry points
            }
        }
    }
}
