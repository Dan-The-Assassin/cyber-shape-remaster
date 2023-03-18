using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = Constants.Scene;

namespace UI
{
    public class PlayMenu : MonoBehaviour
    {
        public void PlayNormal()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.GameMode, (int) GameMode.Classic);
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.CurrentScene) 
                && PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentScene) != (int) Scene.MainMenuScene 
                && PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentScene) != (int) Scene.OptionsMenu 
                && PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentScene) != (int) Scene.GameOverMenu
                && PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentScene) != (int) Scene.PlayMenu
                && PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentScene) != (int) Scene.LevelSelector)
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentScene));
            } 
            else 
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentScene, (int) Scene.Level1);
                SceneManager.LoadScene((int) Scene.Level1);
            }
        }

        public void PlayEndless()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.GameMode, (int) GameMode.Endless);
            SceneManager.LoadScene((int) Scene.LevelSelector);
        }

        public void Back()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.GameMode, (int) GameMode.Classic);
            SceneManager.LoadScene((int) Scene.MainMenuScene);
        }
    }
}

