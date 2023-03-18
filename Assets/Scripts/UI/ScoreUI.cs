using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        private TextMeshProUGUI _scoreText;

        private void Awake()
        {
            _scoreText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.HighScore))
            {
                UpdateScore(0, PlayerPrefs.GetInt(PlayerPrefsKeys.HighScore));
            }
            else
            {
                UpdateScore(0, 0);
            }
        }

        public void UpdateScore(int newScore, int highScore)
        {
            _scoreText.text = PlayerPrefs.GetInt(PlayerPrefsKeys.GameMode) == (int) GameMode.Classic
                ? $"Score: {newScore}\nHighScore: {highScore}"
                : $"Score: {newScore}";
        }
    }
}