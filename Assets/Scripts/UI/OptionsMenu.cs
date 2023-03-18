using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = Constants.Scene;

namespace UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private VolumeSetter volumeSetter;
        [SerializeField] private Slider musicVolume;

        private void Start()
        {
            volumeSetter = GetComponent<VolumeSetter>();
            musicVolume.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume);
        }
        
        private void Update()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, musicVolume.value);
            volumeSetter.BackgroundMusic.volume = PlayerPrefs.GetInt(PlayerPrefsKeys.MusicState) == 1
                ? PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume)
                : 0.0f;
        }

        public void OnMusic()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.MusicState, 1);
            volumeSetter.BackgroundMusic.volume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume);
        }

        public void OffMusic()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicState, 0);
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, 0.0f);
            volumeSetter.BackgroundMusic.volume = 0.0f;
        }

        public void Back()
        {
            SceneManager.LoadScene((int) Scene.MainMenuScene);
        }

        /// Pentru cand vom implementa si sunetele gloantelor si ale transformarilor ( Al doilea buton de optiuni):
        // public void OnSfx()
        // public void OffSfx()
    }
}
