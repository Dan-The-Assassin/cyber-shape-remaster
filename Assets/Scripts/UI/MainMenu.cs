using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = Constants.Scene;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {

        private Canvas _canvas;
        private GameObject _playButton;
        private GameObject _optionsButton;
        private GameObject _instructionsButton;
        private GameObject _exitButton;
        private GameObject _instructionsPanel;
        private GameObject _exitInstructions;
        private void Start()
        {

            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            _playButton = _canvas.gameObject.transform.Find("PlayGame").gameObject;
            _optionsButton = _canvas.gameObject.transform.Find("Options").gameObject;
            _instructionsButton = _canvas.gameObject.transform.Find("Instructions").gameObject;
            _exitButton = _canvas.gameObject.transform.Find("Exit").gameObject;
            _instructionsPanel = _canvas.gameObject.transform.Find("InstructionsPanel").gameObject;
            _exitInstructions = _canvas.gameObject.transform.Find("ExitInstructions").gameObject;
            
            PlayerPrefs.SetInt(PlayerPrefsKeys.GameMode, (int) GameMode.Classic);
            if(!PlayerPrefs.HasKey(PlayerPrefsKeys.GameProgress))
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.GameProgress, 0);
            }
        }
        public void Play()
        {
            SceneManager.LoadScene((int) Scene.PlayMenu);
        }

        public void GoToOptions()
        {
            SceneManager.LoadScene((int) Scene.OptionsMenu);
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }

        public void DisplayInstructions()
        {
            _playButton.gameObject.SetActive(false);
            _optionsButton.gameObject.SetActive(false);
            _instructionsButton.gameObject.SetActive(false);
            _exitButton.gameObject.SetActive(false);
            
            _instructionsPanel.gameObject.SetActive(true);
            _exitInstructions.gameObject.SetActive(true);
        }

        public void ExitInstructions()
        {
            _instructionsPanel.gameObject.SetActive(false);
            _exitInstructions.gameObject.SetActive(false);
            
            _playButton.gameObject.SetActive(true);
            _optionsButton.gameObject.SetActive(true);
            _instructionsButton.gameObject.SetActive(true);
            _exitButton.gameObject.SetActive(true);
        }
    }
}

