using System;
using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Scene = Constants.Scene;

namespace UI
{
    public class LevelSelectorMenu : MonoBehaviour
    {
        [SerializeField] private GameObject err;
        [SerializeField] private GameObject resumeButton;

        private void Start()
        {
            resumeButton.SetActive(PlayerPrefs.GetInt(PlayerPrefsKeys.EndlessInProgress) != 0);
        }

        public void LoadLevel1()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameProgress) < 20)
            {
                StartCoroutine(ShowMessage());
            }
            else
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.LastLevel, (int) Scene.Level1);
                SceneManager.LoadScene((int) Scene.Level1);
            }
        }

        public void LoadLevel2()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameProgress) < 40)
            {
                StartCoroutine(ShowMessage());
            }
            else
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.LastLevel, (int) Scene.Level2);
                SceneManager.LoadScene((int) Scene.Level2);
            }
        }

        public void LoadLevel3()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameProgress) < 60)
            {
                StartCoroutine(ShowMessage());
            }
            else
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.LastLevel, (int) Scene.Level3);
                SceneManager.LoadScene((int) Scene.Level3);
            }
        }

        public void LoadLevel4()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameProgress) < 80)
            {
                StartCoroutine(ShowMessage());
            }
            else
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.LastLevel, (int) Scene.Level4);
                SceneManager.LoadScene((int) Scene.Level4);
            }
        }

        public void Resume()
        {
            GameManager.ShouldLoad = true;
            var savedState = EndlessState.Load();
            SceneManager.LoadScene((int) savedState.Level);
        }

        public void Back()
        {
            SceneManager.LoadScene((int) Scene.PlayMenu);
        }

        private IEnumerator ShowMessage()
        {
            err.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            err.SetActive(false);
        }
    }
}