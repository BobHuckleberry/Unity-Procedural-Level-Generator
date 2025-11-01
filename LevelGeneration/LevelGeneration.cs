using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Threading;
using Unity.VisualScripting;
using JetBrains.Annotations;
using Unity.AI.Navigation;

public class LevelGeneration : MonoBehaviour
{
    public static LevelGeneration Instance { get; private set; }

    [SerializeField] private GameObject entrance;
    [SerializeField] private List<GameObject> rooms;
    [SerializeField] private List<GameObject> specialRooms;
    [SerializeField] private List<GameObject> hallways;
    [SerializeField] private int baseRoomCount;
    [SerializeField] private LayerMask roomsLayermask;
    [SerializeField] private GameObject goalRoomDoor;
    [SerializeField] private LevelLogic levelLogic;
    [SerializeField] private LevelData levelData;
    [SerializeField] private NavMeshSurface navSurface;

    private List<DungeonPart> generatedRooms;

    private int levelCount;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        levelCount = levelData.LevelCount;
    }

    public void Start()
    {
        generatedRooms = new List<DungeonPart>();
        StartGeneration();
    }
    


    public void StartGeneration()
    {
        generatedRooms.Clear();
        Queue<DungeonPart> roomQueue = new Queue<DungeonPart>();
        
        // Instantiate entrance
        GameObject entranceRoom = Instantiate(entrance, transform.position, transform.rotation);
        if (entranceRoom.TryGetComponent<DungeonPart>(out DungeonPart entrancePart))
        {
            generatedRooms.Add(entrancePart);
            roomQueue.Enqueue(entrancePart); // starts the queue with the entrance
        }

        int maxRoomCount = baseRoomCount + (levelCount * 5);
        int roomsPlaced = 0;
        int maxAttempts = maxRoomCount * 20;

        while (roomsPlaced < maxRoomCount && roomQueue.Count > 0 && maxAttempts > 0)
        {
            DungeonPart parentRoom = roomQueue.Dequeue();
            List<Transform> availableParentEntries = parentRoom.GetAvailableEntryPoints();

            foreach (Transform parentEntry in availableParentEntries.ToList())
            {
                if (roomsPlaced >= maxRoomCount) break;
                if (maxAttempts <= 0) break;

                maxAttempts--;

                // Randomly choose room/hallway
                bool placeHallway = Random.value < 0.3f;
                List<GameObject> partList = placeHallway ? hallways : rooms;
                GameObject prefabToPlace = partList[Random.Range(0, partList.Count)];
                if (roomsPlaced == maxRoomCount - 1) prefabToPlace = specialRooms[0];

                // Instantiate new part
                GameObject newPartObj = Instantiate(prefabToPlace, parentEntry.position, Quaternion.identity);
                DungeonPart newPart = newPartObj.GetComponent<DungeonPart>();

                if (newPart == null || newPart.GetAvailableEntryPoints().Count == 0)
                {
                    Destroy(newPartObj);
                    continue;
                }

                // Randomly select new parts entry point
                List<Transform> newPartEntries = newPart.GetAvailableEntryPoints();
                Transform newPartEntry = newPartEntries[Random.Range(0, newPartEntries.Count)];

                // Align rooms
                AlignRooms(parentRoom.transform, newPart.transform, parentEntry, newPartEntry);

                // Check for collisions (ignore parent room's collider)
                if (CheckOverlap(newPart))
                {
                    Destroy(newPartObj);
                    continue;
                }

                // Mark entry points as used
                parentRoom.UseEntryPoint(parentEntry);
                newPart.UseEntryPoint(newPartEntry);

                generatedRooms.Add(newPart);
                roomQueue.Enqueue(newPart); // Add to queue for BFS
                roomsPlaced++;
                //Debug.Log($"Placed room {roomsPlaced}/{maxRoomCount}");
            }
        }

        // Fill unused doors
        foreach (DungeonPart part in generatedRooms)
            part.FillEmptyDoors();
        navSurface.BuildNavMesh();
        Debug.Log($"Generation completed. Rooms placed: {roomsPlaced}");
        Debug.Log($"Count: {levelData.LevelCount}");

    }

    private bool CheckOverlap(DungeonPart newPart)
    {
        Collider newCollider = newPart.GetComponent<Collider>();
        if (newCollider == null) return false;

        // Get all overlapping colliders except self
        Collider[] hits = Physics.OverlapBox(
            newCollider.bounds.center,
            newCollider.bounds.extents * 0.999f, // Slightly shrink bounds to avoid edge overlaps
            newPart.transform.rotation,
            roomsLayermask
        );

        foreach (Collider hit in hits)
        {
            if (hit.transform == newPart.transform)
                continue;

            Debug.LogWarning($"Overlap with {hit.name}");
            return true;
        }
        return false;
    }

    private void AlignRooms(Transform parent, Transform newRoom, Transform parentEntry, Transform newRoomEntry)
    {
        // Align rotation
        Vector3 direction = -parentEntry.forward;
        float angle = Vector3.SignedAngle(newRoomEntry.forward, direction, Vector3.up);
        newRoom.RotateAround(newRoomEntry.position, Vector3.up, angle);

        // Align position
        Vector3 offset = parentEntry.position - newRoomEntry.position;
        newRoom.position += offset;

        Physics.SyncTransforms(); // Force physics update
    }
    
}
