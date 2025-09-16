using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private Slider sfxsVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeText;

    private void OnEnable()
    {
        UpdateVolumeText();
    }

    public void ChangeMusicVolume()
    {
        AudioManager.Instance?.ChangeVolume(SoundsType.Music, (float) Math.Round(musicVolumeSlider.value , 2));
        UpdateVolumeText();
    }

    public void ChangeSFXsVolume()
    {
        AudioManager.Instance?.ChangeVolume(SoundsType.Sfxs, (float) Math.Round(sfxsVolumeSlider.value , 2));
        UpdateVolumeText();
    }

    private void UpdateVolumeText()
    {
        if (AudioManager.Instance)
        { 
            var musicVolume = AudioManager.Instance.GetMusicVolume();
            var sfxsVolume = AudioManager.Instance.GetSFXsVolume();
            musicVolumeText.text = musicVolume == 0f ? "Mute" : musicVolume == 1f ? "Max" : musicVolume.ToString();
            sfxVolumeText.text = sfxsVolume == 0f ? "Mute" : sfxsVolume == 1f ? "Max" : sfxsVolume.ToString();
        }
    }
}
