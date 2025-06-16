using UnityEngine;
[CreateAssetMenu(fileName = "NewTileData", menuName = "Gameplay/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Prefab")]
    [Tooltip("Префаб")]
    public TileController tilePrefab;
    
    [Header("Identity")]
    [Tooltip("Уникальный строковый идентификатор")]
    public string tileID;
    
    [Tooltip("Спрайт формы")]
    public Sprite shapeSprite;
    
    [Tooltip("Спрайт животного, который будет наложен поверх формы.")]
    public Sprite animalSprite;

    [Tooltip("Цвет, в который будет окрашена рамка фигурки.")]
    public Color frameColor;

    [Header("Специальные свойства")]
    [Tooltip("Если true, фигурка будет тяжелее и падать быстрее.")]
    public bool isHeavy = false;
}