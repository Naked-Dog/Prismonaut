using UnityEngine;

public class AudioEvents : MonoBehaviour
{

    public void PlayBoosRoomStartZoneSound()
    {
        AudioManager.Instance?.Play2DSound(LevelEventsSoundsEnum.BossStartZone, true);
    }

    public void StopBossRoomStartZoneSound()
    { 
        AudioManager.Instance?.Stop(LevelEventsSoundsEnum.BossStartZone);
    }

    public void PlayBossRoomMusic()
    {
        AudioManager.Instance?.PlayMusic(MusicEnum.BossRoom);
    }

    public void PlayFinalCinematicMusic()
    {
        AudioManager.Instance?.PlayMusic(MusicEnum.FinalCinematic);
    }
}
