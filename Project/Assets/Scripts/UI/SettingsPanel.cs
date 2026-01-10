using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private Slider sfxsVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeText;

    private void OnEnable()
    {
        UpdateVolumeUI();
    }

    public void ChangeMusicVolume()
    {
        AudioManager.Instance?.ChangeVolume(SoundsType.Music, (float)Math.Round(musicVolumeSlider.value, 2));
        UpdateVolumeUI();
    }

    public void ChangeSFXsVolume()
    {
        AudioManager.Instance?.ChangeVolume(SoundsType.Sfxs, (float)Math.Round(sfxsVolumeSlider.value, 2));
        UpdateVolumeUI();
    }

    private void UpdateVolumeUI()
    {
        if (AudioManager.Instance)
        {
            var musicVolume = AudioManager.Instance.GetMusicVolume();
            var sfxsVolume = AudioManager.Instance.GetSFXsVolume();
            musicVolumeSlider.SetValueWithoutNotify(musicVolume);
            sfxsVolumeSlider.SetValueWithoutNotify(sfxsVolume);
            musicVolumeText.text = musicVolume == 0f ? "Mute" : musicVolume == 1f ? "Max" : musicVolume.ToString();
            sfxVolumeText.text = sfxsVolume == 0f ? "Mute" : sfxsVolume == 1f ? "Max" : sfxsVolume.ToString();
        }
    }
}
