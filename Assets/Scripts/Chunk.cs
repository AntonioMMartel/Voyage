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

    void Generate()
    {
        int cubeCount = Mathf.FloorToInt((chunkSize * chunkSize) / (spacing * spacing));
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = Random.Range(0, chunkSize);
            float randomY = Random.Range(-chunkHeight, chunkHeight);
            float randomZ = Random.Range(0, chunkSize);

            Vector3 pos = new Vector3(
                coord.x * chunkSize + randomX,
                randomY,
                coord.y * chunkSize + randomZ
            );

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            float targetScale = Random.Range(2f, 12f);

            // Start at zero scale
            cube.transform.localScale = Vector3.zero;

            // Add effect
            var effect = cube.AddComponent<ScaleInEffect>();
            effect.Play(targetScale);

            cube.transform.position = pos;
            cube.transform.rotation = Random.rotation;
            cube.transform.SetParent(parentObject.transform);
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(parentObject);
    }
}
