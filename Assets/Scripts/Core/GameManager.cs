using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Core Components")]
    [Tooltip("Ссылка на генератор уровней")]
    [SerializeField] private LevelGenerator levelGenerator;
    [Tooltip("Ссылка на спаунер фишек")]
    [SerializeField] private TileSpawner tileSpawner;
    [Tooltip("Ссылка на контроллер экшен-бара")]
    [SerializeField] private ActionBarController actionBar;

    [Header("UI Screens")]
    [Tooltip("Экран, который показывается при победе")]
    [SerializeField] private GameObject winScreen;
    [Tooltip("Экран, который показывается при проигрыше")]
    [SerializeField] private GameObject loseScreen;
    
    private readonly List<TileController> _activeTilesOnField = new List<TileController>();
    
    private Coroutine _spawnCoroutine;
    
    private bool _isGameActive = false;

    private void OnEnable()
    {
        EventManager.AddListener(GameEvent.TileSpawned, OnTileSpawned);
        EventManager.AddListener(GameEvent.TileClicked, OnTileClicked);
        EventManager.AddListener(GameEvent.LevelWon, OnLevelWon);
        EventManager.AddListener(GameEvent.LevelLost, OnLevelLost);
        EventManager.AddListener(GameEvent.ReshuffleClicked, OnReshuffle);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(GameEvent.TileSpawned, OnTileSpawned);
        EventManager.RemoveListener(GameEvent.TileClicked, OnTileClicked);
        EventManager.RemoveListener(GameEvent.LevelWon, OnLevelWon);
        EventManager.RemoveListener(GameEvent.LevelLost, OnLevelLost);
        EventManager.RemoveListener(GameEvent.ReshuffleClicked, OnReshuffle);
    }
    
    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        _isGameActive = true;
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        CleanupOldLevel();
        actionBar.ResetBar();
        
        var tilesForLevel = levelGenerator.GenerateLevelTiles();
        _spawnCoroutine = StartCoroutine(tileSpawner.SpawnTiles(tilesForLevel));
        
        EventManager.TriggerEvent(GameEvent.LevelStarted);
    }
    
    private void CleanupOldLevel()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }
        
        foreach (var tile in _activeTilesOnField)
        {
            if (tile != null) Destroy(tile.gameObject);
        }
        _activeTilesOnField.Clear();
    }
    
    private void OnTileSpawned(object data) => _activeTilesOnField.Add(data as TileController);
    
    private void OnTileClicked(object data)
    {
        if (!_isGameActive) return;
        
        var tile = data as TileController;
        _activeTilesOnField.Remove(tile);
        
        CheckForWin();
    }

    private void CheckForWin()
    {
        if (_activeTilesOnField.Count == 0 && _isGameActive)
        {
            EventManager.TriggerEvent(GameEvent.LevelWon);
        }
    }
    
    private void OnLevelWon(object data = null)
    {
        if (!_isGameActive) return;
        _isGameActive = false;
        winScreen.SetActive(true);
        Debug.Log("LEVEL WON!");
    }
    
    private void OnLevelLost(object data = null)
    {
        if (!_isGameActive) return;
        _isGameActive = false;
        loseScreen.SetActive(true);
        Debug.Log("LEVEL LOST!");
    }
    
    public void OnReshuffleButtonPressed()
    {
        EventManager.TriggerEvent(GameEvent.ReshuffleClicked);
    }
    
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnReshuffle(object data = null)
    {
        if (!_isGameActive) return;
        Debug.Log("Reshuffling level...");
        StartLevel();
    }
}