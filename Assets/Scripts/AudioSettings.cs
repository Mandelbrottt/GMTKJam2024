using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public float master_volume = 0.9f;
    public float ui_volume = 0.9f;
    public float player_volume = 0.9f;
    public float environment_volume = 0.9f;
    public float music_volume = 0.9f;

    [SerializeField] Slider master_slider;
    [SerializeField] Slider ui_slider;
    [SerializeField] Slider player_slider;
    [SerializeField] Slider environment_slider;
    [SerializeField] Slider music_slider;

    private void Update()
    {
        master_volume = master_slider.value;
        ui_volume = ui_slider.value;
        player_volume = player_slider.value;
        environment_volume = environment_slider.value;
        music_volume = music_slider.value;


        MasterVolumeLevel();
        UIVolumeLevel();
        PlayerVolumeLevel();
        EnvironmentVolumeLevel();
        MusicVolumeLevel();

    }

    public void MasterVolumeLevel()
    {
        SetParameterValue("master_volume", master_slider.value);
    }
    public void UIVolumeLevel()
    {
        SetParameterValue("ui_volume", ui_slider.value);
    }
    public void PlayerVolumeLevel()
    {
        SetParameterValue("player_volume", player_slider.value);
    }
    public void EnvironmentVolumeLevel()
    {
        SetParameterValue("environment_volume", environment_slider.value);
    }
    public void MusicVolumeLevel()
    {
        SetParameterValue("music_volume", music_slider.value);
    }

    float SetParameterValue(string paramName, float paramValue)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(paramName, paramValue);

        return paramValue;
    }
}
