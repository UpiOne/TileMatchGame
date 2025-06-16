using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    
    [Tooltip("Точка в пространстве, вокруг которой будут появляться фишки.")]
    [SerializeField] private Transform spawnPoint;
    
    [Tooltip("Задержка в секундах между появлением каждой новой фишки.")]
    [SerializeField] private float spawnDelay = 0.03f;
    
    [Tooltip("Радиус вокруг точки спауна, в котором могут появляться фишки. Добавляет случайности в падение.")]
    [SerializeField] private float spawnRadius = 2.5f;

    public IEnumerator SpawnTiles(List<TileData> tilesToSpawn)
    {
        foreach (var tileData in tilesToSpawn)
        {
            if (tileData.tilePrefab == null)
            {
                Debug.LogError($"У TileData '{tileData.name}' не назначен префаб!");
                continue;
            }

            Vector3 position = spawnPoint.position + (Vector3)(Random.insideUnitCircle * spawnRadius);
            TileController newTile = Instantiate(tileData.tilePrefab, position, Quaternion.identity, transform);
            newTile.Initialize(tileData);
            EventManager.TriggerEvent(GameEvent.TileSpawned, newTile);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}