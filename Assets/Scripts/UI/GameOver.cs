using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Scene = Constants.Scene;

namespace UI
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameMode) == (int) GameMode.Classic)
            {
                if (PlayerPrefs.HasKey(PlayerPrefsKeys.HighScore))
                {
                    scoreText.text = "High Score: " + PlayerPrefs.GetInt(PlayerPrefsKeys.HighScore);
                }
                else
                {
                    scoreText.text = "High Score: 0";
                }
            }
            else
            {
                int lastLevel = PlayerPrefs.GetInt(PlayerPrefsKeys.LastLevel);
                switch(lastLevel)
                {
                    case 5:
                        if (PlayerPrefs.HasKey(PlayerPrefsKeys.BestSprint1))
                        {
                            scoreText.text = "Best Level Score: " + PlayerPrefs.GetInt(PlayerPrefsKeys.BestSprint1);
                        }
                        else
                        {
                            scoreText.text = "Round Score: 0";
                        }
                        break;
                    case 6:
                        if (PlayerPrefs.HasKey(PlayerPrefsKeys.BestSprint2))
                        {
                            scoreText.text = "Best Level Score: " + PlayerPrefs.GetInt(PlayerPrefsKeys.BestSprint2);
                        }
                        else
                        {
                            scoreText.text = "Round Score: 0";
                        }
                        break;
                    case 7:
                        if (PlayerPrefs.HasKey(PlayerPrefsKeys.BestSprint3))
                        {
                            scoreText.text = "Best Level Score: " + PlayerPrefs.GetInt(PlayerPrefsKeys.BestSprint3);
                        }
                        else
                        {
                            scoreText.text = "Round Score: 0";
                        }
                        break;
                    case 8:
                        if (PlayerPrefs.HasKey(PlayerPrefsKeys.BestSprint4))
                        {
                            scoreText.text = "Best Level Score: " + PlayerPrefs.GetInt(PlayerPrefsKeys.BestSprint4);
                        }
                        else
                        {
                            scoreText.text = "Round Score: 0";
                        }
                        break;
                    default:
                        scoreText.text = "Round Score: 0";
                        break;

                }
                
            }
        }

        public void RestartLevel()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameMode) == (int) GameMode.Classic)
            {
                if (PlayerPrefs.HasKey(PlayerPrefsKeys.CurrentScene))
                {
                    SceneManager.LoadScene(PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentScene));
                }
                else
                {
                    SceneManager.LoadScene((int) Scene.Level1);
                }
            }
            else // Endless
            {
                if (PlayerPrefs.HasKey(PlayerPrefsKeys.LastLevel))
                {
                    SceneManager.LoadScene(PlayerPrefs.GetInt(PlayerPrefsKeys.LastLevel));
                }
                else
                {
                    SceneManager.LoadScene((int) Scene.Level1);
                }
            }
        }

        public void MainMenu()
        {
            SceneManager.LoadScene((int) Scene.MainMenuScene);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}