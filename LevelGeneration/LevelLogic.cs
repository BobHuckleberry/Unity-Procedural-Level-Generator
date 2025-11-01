using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLogic : MonoBehaviour
{
    // Public variable to hold the current level count
    
    [SerializeField] private LevelData levelData;
    public static LevelLogic Instance { get; private set; }
    // Subscribe to the sceneLoaded event when the script is enabled
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unsubscribe from the sceneLoaded event when the script is disabled
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called every time a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelData.LevelCount++;  // Increase the counter
        Debug.Log("Scene loaded: " + scene.name + " | Level count: " + levelData.LevelCount);


        // Here you could also update the UI or trigger other events, if needed.
    }
}