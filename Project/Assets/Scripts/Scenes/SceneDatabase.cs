using UnityEngine;
using System.Collections.Generic;

public enum SceneType
{
    MainMenu,
    StartCinematic,
    Level1,
    LevelBoss,
    FinalScene
}

[CreateAssetMenu(menuName = "Game/Scene Database")]
public class SceneDatabase : ScriptableObject
{
    [System.Serializable]
    public class SceneEntry
    {
        public SceneType Type;
        public string SceneName;
        public MusicEnum MusicKey;
    }

    public List<SceneEntry> Scenes = new();

    public SceneEntry GetEntry(SceneType type)
    {
        return Scenes.Find(s => s.Type == type);
    }
}
