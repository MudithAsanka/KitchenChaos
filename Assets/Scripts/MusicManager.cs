using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;

    private float volume = 0.3f;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);
        audioSource.volume = volume;
    }

    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 1f)
        {
            // Back to volume 0
            volume = 0f;
        }
        audioSource.volume = volume;

        // Set music volume in PlayerPrefs
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        // Unity automatically save PlayerPrefs but in case of crash or something make sure to manually save
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
