using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] Transform player;

    [Header("Chunk Settings")]
    [SerializeField] float chunkSize = 50f;
    [SerializeField] float spacing = 5f;

    [SerializeField] int clusterCount = 8;
    [SerializeField] int elementsPerCluster = 20;
    [SerializeField] float clusterRadius = 60f;

    Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    [Header("Noise Settings")]
    [SerializeField] float noiseScale = 0.01f;
    [SerializeField] float threshold = 0.55f;

    void Update()
    {
        UpdateChunks();
    }

    void UpdateChunks()
    {
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

        // 7x7 grid
        for (int x = -3; x <= 3; x++)
        {
            for (int z = -3; z <= 3; z++)
            {
                Vector2Int coord = new Vector2Int(
                    playerChunk.x + x,
                    playerChunk.y + z
                );

                neededChunks.Add(coord);

                if (!chunks.ContainsKey(coord))
                {
                    Chunk newChunk = new Chunk(
                        coord,
                        chunkSize,
                        spacing,
                        clusterCount,
                        elementsPerCluster,
                        clusterRadius,
                        noiseScale,
                        threshold
                    );
                    chunks.Add(coord, newChunk);
                }
            }
        }

        // Remove unused chunks
        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var chunk in chunks)
        {
            if (!neededChunks.Contains(chunk.Key))
            {
                chunk.Value.Destroy();
                toRemove.Add(chunk.Key);
            }
        }

        foreach (var coord in toRemove)
        {
            chunks.Remove(coord);
        }
    }


}
