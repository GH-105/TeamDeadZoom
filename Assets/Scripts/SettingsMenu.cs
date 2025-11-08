using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public AudioMixer mainMixer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustMasterVol()
    {
        mainMixer.SetFloat("MasterVolume", masterVolume.value);
    }
    public void AdjustMusicVol()
    {
        mainMixer.SetFloat("MusicVolume", musicVolume.value);
    }
    public void AdjustSFXVol()
    {
        mainMixer.SetFloat("SFXVolume", sfxVolume.value);
    }
}
