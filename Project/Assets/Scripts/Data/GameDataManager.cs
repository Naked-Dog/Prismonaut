using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private PlayerGameData playerGameData;
    private FileDataHandler dataHandler;
    public static GameDataManager Instance {get; private set;}

    protected void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Game Data Manager alredy exists");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, "PlayerGameData.json");
        LoadGame();
    }

    public void NewGame()
    {
        playerGameData = new PlayerGameData();
    }

    public void SaveGame()
    {
        dataHandler.Save(playerGameData);
    }

    public void LoadGame()
    {
        playerGameData = dataHandler.Load();
    }

    public void SavePlayerPosition(Vector3 position)
    {
        playerGameData.playerPosition = position;
        SaveGame();
    }

    public Vector3 GetSavedPlayerPosition()
    {
        return playerGameData.playerPosition;
    }
}
