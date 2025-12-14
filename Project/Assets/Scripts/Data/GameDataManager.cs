using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private PlayerGameData playerGameData;
    private FileDataHandler dataHandler;
    public static GameDataManager Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, "PlayerGameData.json");
        LoadGame();
    }

    public void NewGame()
    {
        playerGameData = new PlayerGameData();
    }

    public void SaveGame()
    {
        if (dataHandler == null)
        {
            return;
        }

        if (playerGameData == null)
        {
            NewGame();
        }
        dataHandler.Save(playerGameData);
    }

    public void LoadGame()
    {
        playerGameData = dataHandler.Load();
        if (playerGameData == null)
        {
            NewGame();
        }
    }

    public void SavePlayerPosition(Vector3 position)
    {
        if (playerGameData == null)
        {
            NewGame();
        }
        playerGameData.playerPosition = position;
        SaveGame();
    }

    public Vector3 GetSavedPlayerPosition()
    {
        return playerGameData.playerPosition;
    }
}
