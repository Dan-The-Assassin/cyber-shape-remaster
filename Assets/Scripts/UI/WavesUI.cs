using Constants;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WavesUI : MonoBehaviour
    {
        private TextMeshProUGUI _wavesText;

        private void Awake()
        {
            _wavesText = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateWaves(int remainingWaves)
        {
            _wavesText.text = PlayerPrefs.GetInt(PlayerPrefsKeys.GameMode) == (int) GameMode.Classic
                ? $"Remaining Waves: {remainingWaves}"
                : "Endless Mode Active";
        }
    }
}
