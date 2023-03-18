using Constants;
using UnityEngine;

public class VolumeSetter : MonoBehaviour
{
    [field: SerializeField] public AudioSource BackgroundMusic { get; private set; }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.MusicVolume))
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, 0.5f);
        }

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.MusicState))
        {
            BackgroundMusic.volume = PlayerPrefs.GetInt(PlayerPrefsKeys.MusicState) == 1
                ? PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume)
                : 0.0f;
        }
        else
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.MusicState, 1);
            BackgroundMusic.volume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume);
        }
    }
}