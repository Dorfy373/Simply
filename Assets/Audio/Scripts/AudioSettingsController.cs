using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Settings")]
    [SerializeField] private string _musicVolumeParameter = "MusicVolume";

    public void SetMusicVolume(float value)
    {
        if (value <= 0.0001f)
        {
            
            _audioMixer.SetFloat(_musicVolumeParameter, -80f);
            return;
        }

        float volume = Mathf.Log10(value) * 20f;

        _audioMixer.SetFloat(_musicVolumeParameter, volume);
    }
}
