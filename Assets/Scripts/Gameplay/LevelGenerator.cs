using System.Collections.Generic;
using UnityEngine;
public class LevelGenerator : MonoBehaviour
{
    [Tooltip("Список всех возможных типов фигурок. Перетащите сюда все TileData ассеты из проекта.")]
    [SerializeField] private List<TileData> allTileTypes;
    
    [Header("Level Configuration")]
    [Tooltip("Сколько уникальных видов фигурок будет на уровне.")]
    [Range(1, 15)]
    [SerializeField] private int uniqueTypesPerLevel = 8;
    
    [Tooltip("Сколько наборов по 3 штуки будет для каждого вида. Общее кол-во фишек = uniqueTypes * setsPerType * 3.")]
    [Range(1, 10)]
    [SerializeField] private int setsPerType = 4;
    
    public List<TileData> GenerateLevelTiles()
    {
        var levelTiles = new List<TileData>();
        var availableTypes = new List<TileData>(allTileTypes);
        var selectedTypes = new List<TileData>();
        int typesToSelect = Mathf.Min(uniqueTypesPerLevel, availableTypes.Count);

        for (int i = 0; i < typesToSelect; i++)
        {
            int randIndex = Random.Range(0, availableTypes.Count);
            selectedTypes.Add(availableTypes[randIndex]);
            availableTypes.RemoveAt(randIndex);
        }
        
        foreach (var tileType in selectedTypes)
        {
            for (int i = 0; i < setsPerType * 3; i++)
            {
                levelTiles.Add(tileType);
            }
        }
        
        for (int i = levelTiles.Count - 1; i > 0; i--) {
            int randomIndex = Random.Range(0, i + 1);
            (levelTiles[i], levelTiles[randomIndex]) = (levelTiles[randomIndex], levelTiles[i]);
        }

        Debug.Log($"Generated level with {levelTiles.Count} tiles ({selectedTypes.Count} unique types).");
        return levelTiles;
    }
}