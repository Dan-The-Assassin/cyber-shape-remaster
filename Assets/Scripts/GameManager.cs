using System.Collections;
using System.Linq;
using Constants;
using Enemy;
using Evolution;
using TMPro;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Scene = Constants.Scene;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool withEnemyWaves = true;
    [SerializeField] private Texture2D crosshairImg;
    [FormerlySerializedAs("panel")] [SerializeField] private GameObject pausePanel;

    [Header("Enemy settings")] [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField] private float maxWidth;
    [SerializeField] private float secondsTillSpawn = 3f;
    [SerializeField] private float percentToNextWave = .75f;

    [Header("Level settings")] [SerializeField]
    private int waves = 3;

    [SerializeField] private int enemiesPerWave = 5;

    [Header("Pickups")]
    [FormerlySerializedAs("_hpPickup")] [SerializeField] private GameObject hpPickup;
    [FormerlySerializedAs("_dmgPickup")] [SerializeField] private GameObject dmgPickup;
    [FormerlySerializedAs("_shieldPickup")] [SerializeField] private GameObject shieldPickup;
    [SerializeField] public int PickupTotal { get; set; }

    public static bool ShouldLoad = false;

    private bool _spawning = false;
    private int _pause = 0;
    private int _waveCount = 0;
    private HudManager _hudManager;
    private Player _player;
    private Vector3 _playerPosition;
    private Vector3 _floorSize;
    private Camera _camera;
    private int _levelIndex;
    private int _pickupCount;
    private TextMeshProUGUI _gameWonText;
    private GameObject _goToMenuButton;
    public bool isGameWon = false;
    private (bool, Vector3)[] _vectorOfPositions1 = new[] {
            (false, new Vector3(-13.0f, 0.5f, 2.4f)),
            (false, new Vector3(-13.2f, 0.5f, 47.7f)),
            (false, new Vector3(-33.4f, 0.5f, 39.9f)),
            (false, new Vector3(42.4f, 0.5f, 41.1f)),
            (false, new Vector3(42.4f, 0.5f, 26.1f)),

            (false, new Vector3(42.4f, 0.5f, 23.9f)),
            (false, new Vector3(42.4f, 0.5f, -28.8f)),
            (false, new Vector3(5.3f, 0.5f, -28.8f)),
            (false, new Vector3(3.0f, 0.5f, -38.3f)),
            (false, new Vector3(-45.3f, 0.5f, -26.2f)),
        };
    private (bool, Vector3)[] _vectorOfPositions2 = new[] {
            (false, new Vector3(0.0f, 0.5f, 27.6f)),
            (false, new Vector3(-13.5f, 0.5f, 27.6f)),
            (false, new Vector3(-24.5f, 0.5f, 42.9f)),
            (false, new Vector3(24.1f,  0.5f, 42.9f)),
            (false, new Vector3(24.1f, 0.5f, 7.5f)),

            (false, new Vector3(24.1f, 0.5f, -26.9f)),
            (false, new Vector3(43.0f, 0.5f, -18.9f)),
            (false, new Vector3(-13.7f, 0.5f, -13.0f)),
            (false, new Vector3(-28.9f, 0.5f, 1.6f)),
            (false, new Vector3(8.6f, 0.5f, -45.4f)),
        };

    private (bool, Vector3)[] _vectorOfPositions3 = new[] {
            (false, new Vector3(18.5f, 0.5f, -36.3f)),
            (false, new Vector3(31.9f, 0.5f, -36.3f)),
            (false, new Vector3(18.8f, 0.5f, -32.0f)),
            (false, new Vector3(-36.0f,  0.5f, -14.5f)),
            (false, new Vector3(-27.2f, 0.5f, 6.5f)),

            (false, new Vector3(-23.0f, 0.5f, 35.4f)),
            (false, new Vector3(-44.5f, 0.5f, 23.3f)),
            (false, new Vector3(-0.8f, 0.5f, 35.5f)),
            (false, new Vector3(8.5f, 0.5f, 7.4f)),
            (false, new Vector3(27.3f, 0.5f, 24.7f)),
        };

    private (bool, Vector3)[] _vectorOfPositions4 = new[] {
            (false, new Vector3(-30.4f, 0.5f, 10.0f)),
            (false, new Vector3(-33.5f, 0.5f, 28.2f)),
            (false, new Vector3(-13.8f, 0.5f, 32.4f)),
            (false, new Vector3(7.3f,  0.5f, 23.5f)),
            (false, new Vector3(35.8f, 0.5f, 38.4f)),

            (false, new Vector3(35.8f, 0.5f, -18.9f)),
            (false, new Vector3(19.7f, 0.5f, -35.8f)),
            (false, new Vector3(-12.4f, 0.5f, -45.9f)),
            (false, new Vector3(-34.3f, 0.5f, -31.7f)),
            (false, new Vector3(-33.4f, 0.5f, -4.8f)),
        };

    private void Awake()
    {
        _levelIndex = SceneManager.GetActiveScene().buildIndex - (int) Scene.Level1;

        _hudManager = GameObject.Find("HUD").GetComponent<HudManager>();
        _player = GetComponentInChildren<Player>();
        _player.UnlockBulletsForLevel(_levelIndex + 1);
        _floorSize = GameObject.FindWithTag("Floor").GetComponent<MeshRenderer>().bounds.size;
        if (SceneManager.GetActiveScene().buildIndex == (int) Scene.Level5)
        {
            _gameWonText = _hudManager.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
            _goToMenuButton = _hudManager.gameObject.transform.Find("GoToMenu").gameObject;
        }

        maxWidth = _floorSize.x / 3;
        _camera = Camera.main;
        PickupTotal = 0;
        _pickupCount = 0;
    }

    private void Start()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.GamePause, _pause);

        var hotSpot = new Vector2(crosshairImg.width / 2f, crosshairImg.height / 2f);
        Cursor.SetCursor(crosshairImg, hotSpot, CursorMode.Auto);

        _player.GetComponent<Evolvable>().Evolve(_levelIndex);

        if (_levelIndex == 4)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.GameProgress, 100);
            // PopUp Msg (" Congrats!!") sau trigger Achivement.
        }

        if (ShouldLoad && PlayerPrefs.GetInt(PlayerPrefsKeys.GameMode) == (int) GameMode.Endless)
        {
            ShouldLoad = false;
            var savedState = EndlessState.Load();
            _player.gameObject.transform.position = savedState.PlayerPosition;
            _player.CurrentHealth = savedState.PlayerHealth;

            foreach (var enemyPosition in savedState.EnemyPositions)
            {
                var enemyObject = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);

                // Evolve to random stage (max is determined by current level).
                var enemyEvolvable = enemyObject.GetComponentInChildren<Evolvable>();
                var stageIndex = Random.Range(0, _levelIndex + 1);
                enemyEvolvable.Evolve(stageIndex);

                _hudManager.FollowEnemy(enemyObject);
            }
            _player.AddScore(savedState.Score);
        }
        else
        {
            if (withEnemyWaves)
            {
                NextWave();
            }
        }
    }

    private void Update()
    {
        _playerPosition = _player.transform.position;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_pause == 0)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        if (PickupTotal < 4)
        {
            SpawnPickup();
        }

        if (!_spawning && withEnemyWaves)
        {
            CheckNoEnemies();
        }
    }

    private void SpawnPickup()
    {
        // Instantiate(_hpPickup, new Vector3(Random.Range(-_floorSize.x/2, _floorSize.x / 2), 1.7f, Random.Range(-_floorSize.z / 2, _floorSize.z / 2)), Quaternion.identity);
        // pickupTotal++;
        switch (_pickupCount)
        {
            case 0:
                Instantiate(hpPickup,
                    new Vector3(Random.Range(-_floorSize.x / 2, _floorSize.x / 2), 1.7f,
                        Random.Range(-_floorSize.z / 2, _floorSize.z / 2)), Quaternion.identity);
                break;
            case 1:
                Instantiate(dmgPickup,
                    new Vector3(Random.Range(-_floorSize.x / 2, _floorSize.x / 2), 1.7f,
                        Random.Range(-_floorSize.z / 2, _floorSize.z / 2)), Quaternion.identity);
                break;
            case 2:
                Instantiate(shieldPickup,
                    new Vector3(Random.Range(-_floorSize.x / 2, _floorSize.x / 2), 1.7f,
                        Random.Range(-_floorSize.z / 2, _floorSize.z / 2)), Quaternion.identity);
                break;
        }

        _pickupCount = (_pickupCount + 1) % 3;
        PickupTotal++;
    }

    private void CheckNoEnemies()
    {
        if (GameObject.FindGameObjectsWithTag(Tags.Enemy).Length < percentToNextWave * enemiesPerWave)
        {
            NextWave();
        }
    }
    
    private Vector3 GetRandomPosition()
    {
        Console.WriteLine("entering the funtion");
        //vector of positions for Map 1
        ref (bool, Vector3)[] _vectorOfPositions = ref _vectorOfPositions1;

        if (_levelIndex == 1) {
            _vectorOfPositions = ref _vectorOfPositions1;
        } else if (_levelIndex == 2) {
                    _vectorOfPositions = ref _vectorOfPositions2;
                } else if (_levelIndex == 3) {
                        _vectorOfPositions = ref _vectorOfPositions3;
                    } else {
                        _vectorOfPositions = ref _vectorOfPositions4;
                    }

        while (true)
        {
            // get random point from the vector of positions
            var randomPositionFromVector = Random.Range(0, 10);
            Console.WriteLine(randomPositionFromVector);
            while (_vectorOfPositions[randomPositionFromVector].Item1 == true)
            {
                randomPositionFromVector = Random.Range(0, 10);
                Console.WriteLine(randomPositionFromVector);
            }

            _vectorOfPositions[randomPositionFromVector].Item1 = true;

            var randomPosition = _vectorOfPositions[randomPositionFromVector].Item2;

            // check if the point is inside the map
            if (randomPosition.x < -_floorSize.x / 2 || randomPosition.x > _floorSize.x / 2 ||
                randomPosition.z < -_floorSize.z / 2 || randomPosition.z > _floorSize.z / 2)
            {
                continue;
            }

            // check if the point is outside camera view
            if (Mathf.Abs(randomPosition.x - _playerPosition.x) < _camera.orthographicSize ||
                Mathf.Abs(randomPosition.z - _playerPosition.z) < _camera.orthographicSize * _camera.aspect)
            {
                continue;
            }

            if (Physics.CheckSphere(randomPosition, 0.7f, 7))
            {
                continue;
            }

            return randomPosition;
        }
    }

    private void NextWave()
    {
        if (_waveCount + 1 > (_levelIndex + 1) * waves)
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameMode) == (int) GameMode.Classic)
            {
                switch (_levelIndex)
                {
                    case 0:
                        PlayerPrefs.SetInt(PlayerPrefsKeys.GameProgress, 20);
                        break;
                    case 1:
                        PlayerPrefs.SetInt(PlayerPrefsKeys.GameProgress, 40);
                        break;
                    case 2:
                        PlayerPrefs.SetInt(PlayerPrefsKeys.GameProgress, 60);
                        break;
                    case 3:
                        PlayerPrefs.SetInt(PlayerPrefsKeys.GameProgress, 80);
                        break;
                }

                ProgressToNextLevel();
            }
            else // Endless
            {
                _waveCount = 1;
            }
        }
        else
        {
            _waveCount++;
            _hudManager.WavesUI.UpdateWaves((_levelIndex + 1) * waves - _waveCount + 1);
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        _spawning = true;
        yield return new WaitForSeconds(secondsTillSpawn);
        for (var i = 0; i < enemiesPerWave; i++)
        {
            var enemyObject = Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity);

            // Evolve to random stage (max is determined by current level).
            var enemyEvolvable = enemyObject.GetComponentInChildren<Evolvable>();
            var stageIndex = Random.Range(0, _levelIndex + 1);
            enemyEvolvable.Evolve(stageIndex);

            _hudManager.FollowEnemy(enemyObject);
        }

        // call reset vector function
        if (_levelIndex == 1){
            ResetVectorOfPositions(_vectorOfPositions1);
        }
        else if (_levelIndex == 2){
            ResetVectorOfPositions(_vectorOfPositions2);
        }
        else if (_levelIndex == 3){
            ResetVectorOfPositions(_vectorOfPositions3);
        }
        else{
            ResetVectorOfPositions(_vectorOfPositions4);
        }

        _spawning = false;
    }

    private void ResetVectorOfPositions((bool, Vector3)[] _vectorOfPos)
    {
        for( var i = 0; i < 10; i++)
        {
            _vectorOfPos[i].Item1 = false;
        }
    }

    private void ProgressToNextLevel()
    {
        var activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (activeSceneIndex < (int) Scene.Level5)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentScene, activeSceneIndex + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void PauseGame()
    {
        _pause = 1;
        PlayerPrefs.SetInt(PlayerPrefsKeys.GamePause, _pause);
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        _pause = 0;
        PlayerPrefs.SetInt(PlayerPrefsKeys.GamePause, _pause);
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void GoBackMain()
    {
        _pause = 0;
        PlayerPrefs.SetInt(PlayerPrefsKeys.GamePause, _pause);

        // Save game state if in endless
        if (PlayerPrefs.GetInt(PlayerPrefsKeys.GameMode) == (int) GameMode.Endless)
        {
            var enemyPositions = FindObjectsOfType<EnemyController>()
                .Select(x => x.gameObject.transform.position).ToArray();
            var gameState = new EndlessState
            {
                Level = (Scene) SceneManager.GetActiveScene().buildIndex,
                Score = _player.Score,
                PlayerHealth = _player.CurrentHealth,
                PlayerPosition = _player.transform.position,
                EnemyPositions = enemyPositions
            };
            gameState.Save();
        }

        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        SceneManager.LoadScene((int) Scene.MainMenuScene);
    }

    public void DisplayGameWonText()
    {
        _gameWonText.gameObject.SetActive(true);
        _goToMenuButton.gameObject.SetActive(true);
        isGameWon = true;
    }
}