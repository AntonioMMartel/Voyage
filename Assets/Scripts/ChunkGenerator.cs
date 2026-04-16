using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] Transform player;
    private HashSet<Vector2Int> neededChunks;

    [Header("Chunk Settings")]
    [SerializeField] float chunkSize = 50f;
    [SerializeField] float spacing = 5f;

    [SerializeField] int clusterCount = 8;
    [SerializeField] int elementsPerCluster = 20;
    [SerializeField] float clusterRadius = 60f;

    Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    [SerializeField] GameObject[] elements, elementsGeneric, elementsTerrain;
    [SerializeField] float elementTypeNoiseScale = 0.008f;

    [Header("Noise Settings")]
    [SerializeField] float noiseScale = 0.01f;
    [SerializeField] float threshold = 0.55f;

    private void Start()
    {
        LoadChunks();
    }
    void Update()
    {
        UpdateChunks();
    }


    void LoadChunks()
    {
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        this.neededChunks = new HashSet<Vector2Int>();

        for (int x = -12; x <= 12; x++)
        {
            for (int z = -10; z <= 10; z++)
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
                        threshold,
                        elements,
                        elementTypeNoiseScale,
                        false,
                        elementsGeneric,
                        elementsTerrain
                    );


                    chunks.Add(coord, newChunk);
                }
            }
        }


        /*
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
        */
    }
    void UpdateChunks()
    {
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

        for (int x = -5; x <= 5; x++)
        {
            for (int z = -5; z <= 5; z++)
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
                        threshold,
                        elements,
                        elementTypeNoiseScale,
                        true,
                        elementsGeneric,
                        elementsTerrain
                    );
                    chunks.Add(coord, newChunk);
                }
            }
        }
    }
}
