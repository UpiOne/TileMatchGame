using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class TileController : MonoBehaviour
{
    [Header("Visual Components")]
    [Tooltip("Ссылка на SpriteRenderer для рамки")]
    [SerializeField] private SpriteRenderer frameRenderer;
    [Tooltip("Ссылка на SpriteRenderer для формы")]
    [SerializeField] private SpriteRenderer shapeRenderer;
    [Tooltip("Ссылка на SpriteRenderer для животного")]
    [SerializeField] private SpriteRenderer animalRenderer;
    
    public TileData Data { get; private set; }

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private bool _isInteractable = true;
    
    #region Unchanged Methods
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    
    public void Initialize(TileData tileData)
    {
        this.Data = tileData;
        
        frameRenderer.color = Data.frameColor;
        shapeRenderer.sprite = Data.shapeSprite;
        animalRenderer.sprite = Data.animalSprite;
        
        if (Data.isHeavy)
        {
            _rb.mass *= 2.5f;
        }
    }
    
    private void OnMouseDown()
    {
        if (!_isInteractable) return;
        EventManager.TriggerEvent(GameEvent.TileClicked, this);
    }
    #endregion

    public void PrepareForActionBar()
    {
        _isInteractable = false;
        _collider.enabled = false; 
        _rb.isKinematic = true; 
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
    }
    
    public void AnimateToPosition(Vector3 targetPosition, Vector3 targetScale, float duration, Action onComplete) // <-- ИЗМЕНЕНИЕ: Добавлен targetScale
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad));
        sequence.Join(transform.DORotate(Vector3.zero, duration).SetEase(Ease.OutQuad));
        sequence.Join(transform.DOScale(targetScale, duration).SetEase(Ease.OutQuad));
        sequence.OnComplete(() => onComplete?.Invoke());
    }
}