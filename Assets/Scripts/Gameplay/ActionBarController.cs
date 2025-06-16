using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;

public class ActionBarController : MonoBehaviour
{
    private const int MAX_SLOTS = 7;
    
    [Tooltip("Список трансформов 7 слотов в UI, которые служат точками назначения для фишек.")]
    [SerializeField] private List<Transform> slots;
    [Tooltip("Длительность полета фишки от игрового поля до слота в баре.")]
    [SerializeField] private float tileFlyDuration = 0.3f;
    [Tooltip("Длительность анимации перестановки фишек в баре.")]
    [SerializeField] private float rearrangeDuration = 0.2f;
    [Tooltip("Масштаб фишки, когда она находится в баре (1 = 100%).")]
    [SerializeField] private float tileScaleInBar = 0.8f;
    private readonly List<TileController> _tilesInBar = new List<TileController>();
    private bool _isLocked = false;
    
    #region Unchanged Methods
    private void OnEnable()
    { 
        EventManager.AddListener(GameEvent.TileClicked, OnTileClicked);
        EventManager.AddListener(GameEvent.LevelLost, LockBar);
        EventManager.AddListener(GameEvent.LevelWon, LockBar);
        EventManager.AddListener(GameEvent.LevelStarted, ResetBar);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(GameEvent.TileClicked, OnTileClicked);
        EventManager.RemoveListener(GameEvent.LevelLost, LockBar);
        EventManager.RemoveListener(GameEvent.LevelWon, LockBar);
        EventManager.RemoveListener(GameEvent.LevelStarted, ResetBar);
    }
    #endregion
    
    private void OnTileClicked(object data)
    {
        if (_isLocked || _tilesInBar.Count >= MAX_SLOTS) return;
        
        var tile = data as TileController;
        if (tile == null) return;
        
        tile.PrepareForActionBar(); 
        _isLocked = true; 
    
        AddTileToBar(tile);
    }
    
    private void AddTileToBar(TileController tile)
    {
        int insertIndex = _tilesInBar.FindIndex(t => string.CompareOrdinal(t.Data.tileID, tile.Data.tileID) > 0);
        if (insertIndex == -1) 
        {
            insertIndex = _tilesInBar.Count;
        }
    
        _tilesInBar.Insert(insertIndex, tile);
        
        for (int i = 0; i < _tilesInBar.Count; i++)
        {
            if (_tilesInBar[i] == tile)
            {
                continue;
            }
            _tilesInBar[i].transform.position = slots[i].position;
        }
        Vector3 targetScale = Vector3.one * tileScaleInBar;
        tile.AnimateToPosition(slots[insertIndex].position, targetScale, tileFlyDuration, OnTileArrived);
    }
    
    private void OnTileArrived()
    {
        CheckForMatch();
    }
    
    private void CheckForMatch()
    {
        var matchGroup = _tilesInBar.GroupBy(t => t.Data.tileID).FirstOrDefault(g => g.Count() >= 3);

        if (matchGroup != null)
        {
            _isLocked = true; 
            List<TileController> matchedTiles = matchGroup.Take(3).ToList();
            Sequence destroySequence = DOTween.Sequence();
            foreach (var tile in matchedTiles)
            {
                _tilesInBar.Remove(tile);
                destroySequence.Join(tile.transform.DOScale(Vector3.zero, rearrangeDuration).SetEase(Ease.InBack));
                destroySequence.Join(tile.GetComponent<SpriteRenderer>().DOFade(0, rearrangeDuration));
            }
            
            destroySequence.OnComplete(() =>
            {
                foreach (var tile in matchedTiles)
                {
                    Destroy(tile.gameObject);
                }
                EventManager.TriggerEvent(GameEvent.MatchFound, matchGroup.Key);
                UpdateBarVisuals(true, () => CheckForMatch());
            });
        }
        else
        {
            _isLocked = false;
            CheckForLoss();
        }
    }
    
    private void UpdateBarVisuals(bool animated, Action onComplete = null)
    {

        Vector3 targetScale = Vector3.one * tileScaleInBar;

        if (!animated)
        {
            for (int i = 0; i < _tilesInBar.Count; i++)
            {
                _tilesInBar[i].transform.position = slots[i].position;
                _tilesInBar[i].transform.localScale = targetScale;
            }
            onComplete?.Invoke();
            return;
        }
        
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < _tilesInBar.Count; i++)
        {
            sequence.Join(_tilesInBar[i].transform.DOMove(slots[i].position, rearrangeDuration).SetEase(Ease.OutQuad));
            _tilesInBar[i].transform.localScale = targetScale;
        }
        
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    private void CheckForLoss()
    {
        if (_tilesInBar.Count >= MAX_SLOTS)
        {
            EventManager.TriggerEvent(GameEvent.LevelLost);
        }
    }

    private void LockBar(object data = null) => _isLocked = true;
    
    public void ResetBar(object data = null)
    {
        DOTween.Kill(this, true); 
        
        foreach (var tile in _tilesInBar)
        {
            if(tile != null) Destroy(tile.gameObject);
        }
        _tilesInBar.Clear();
        _isLocked = false;
    }
}