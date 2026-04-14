using UnityEngine;

public class Chunk
{
    public Vector2Int coord;
    public GameObject parentObject;


    float chunkSize;
    float spacing;
    float density = 20f;

    public float chunkHeight = 500f;

    public Chunk(Vector2Int coord, float chunkSize, float spacing)
    {
        this.coord = coord;
        this.chunkSize = chunkSize;
        this.spacing = spacing;

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
        float noiseScale = 0.04f;
        float threshold = 0.6f;

        float step = spacing * 5f; // IMPORTANT: controls density & performance

        for (float x = 0; x < chunkSize; x += step)
        {
            for (float y = -chunkHeight; y < chunkHeight; y += step)
            {
                for (float z = 0; z < chunkSize; z += step)
                {
                    float worldX = coord.x * chunkSize + x;
                    float worldY = y;
                    float worldZ = coord.y * chunkSize + z;

                    float noise = Perlin3D(
                        worldX * noiseScale,
                        worldY * noiseScale,
                        worldZ * noiseScale
                    );

                    if (noise < threshold)
                        continue;

                    SpawnCube(worldX, worldY, worldZ, noise);
                }
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
