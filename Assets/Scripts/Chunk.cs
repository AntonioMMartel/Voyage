using UnityEngine;

public class Chunk
{
    public Vector2Int coord;
    public GameObject parentObject;


    private float chunkSize, spacing, clusterCount, asteroidsPerCluster, clusterRadius;
    float density = 20f; 

    public float chunkHeight = 500f;

    public Chunk(Vector2Int coord, float chunkSize, float spacing, float clusterCount, float asteroidsPerCluster, float clusterRadius)
    {
        this.coord = coord;
        this.chunkSize = chunkSize;
        this.spacing = spacing;

        this.clusterCount = clusterCount;
        this.asteroidsPerCluster = asteroidsPerCluster;
        this.clusterRadius = clusterRadius;

        parentObject = new GameObject($"Chunk_{coord.x}_{coord.y}");
        Generate();
    }

    /* Este es el malo
    void Generate()
    {
        float noiseScale = 0.1f;   // controls cluster size
        float threshold = 0.8f;     // controls density (higher = fewer cubes)

        for (float x = 0; x < chunkSize; x += spacing)
        {
            for (float z = 0; z < chunkSize; z += spacing)
            {
                float worldX = coord.x * chunkSize + x;
                float worldZ = coord.y * chunkSize + z;


                // Sample Perlin noise
                float noise = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale);

                if (noise > threshold)
                {
                    float worldY = Mathf.Lerp(-chunkHeight, chunkHeight, noise);
                    Vector3 pos = new Vector3(worldX, worldY, worldZ);

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    float targetScale = Mathf.Lerp(2f, 12f, noise);

                    cube.transform.localScale = Vector3.zero;

                    var effect = cube.AddComponent<ScaleInEffect>();
                    effect.Play(targetScale);

                    cube.transform.position = pos;
                    cube.transform.rotation = Random.rotation;
                    cube.transform.SetParent(parentObject.transform);
                }
            }
        }
    }
    */

    void Generate()
    {
        int clusterCount = 8;          // how many clusters per chunk
        int asteroidsPerCluster = 20;  // density inside cluster
        float clusterRadius = 60f;

        float noiseScale = 0.01f;
        float threshold = 0.55f;

        for (int c = 0; c < clusterCount; c++)
        {
            // Random cluster center inside chunk
            Vector3 center = new Vector3(
                coord.x * chunkSize + Random.Range(0, chunkSize),
                Random.Range(-chunkHeight, chunkHeight),
                coord.y * chunkSize + Random.Range(0, chunkSize)
            );

            // Use noise to decide if this cluster should exist
            float noise = Perlin3D(
                center.x * noiseScale,
                center.y * noiseScale,
                center.z * noiseScale
            );

            if (noise < threshold)
                continue;

            // Spawn asteroids inside cluster
            for (int i = 0; i < asteroidsPerCluster; i++)
            {
                Vector3 offset = Random.insideUnitSphere * clusterRadius;

                // Optional: falloff so edges are less dense
                float falloff = 1f - (offset.magnitude / clusterRadius);

                if (Random.value > falloff)
                    continue;

                Vector3 pos = center + offset;

                SpawnCube(pos.x, pos.y, pos.z, noise);
            }
        }
    }

    float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }

    void SpawnCube(float x, float y, float z, float noise)
    {
        Vector3 pos = new Vector3(x, y, z);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float size = Mathf.Lerp(2f, 8f, noise);

        cube.transform.localScale = Vector3.one * size;
        cube.transform.position = pos;
        cube.transform.rotation = Random.rotation;

        cube.transform.SetParent(parentObject.transform);
    }
    public void Destroy()
    {
        GameObject.Destroy(parentObject);
    }
}
