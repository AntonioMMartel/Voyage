using UnityEngine;
using UnityEngine.UIElements;

public class Chunk
{
    public Vector2Int coord;
    public GameObject parentObject;


    private float chunkSize, spacing, clusterCount, elementsPerCluster, clusterRadius, noiseScale, threshold, typeScale;
    private GameObject[] elements;
    float density = 20f; 

    public float chunkHeight = 500f;


    public Chunk(Vector2Int coord,
        float chunkSize, 
        float spacing, 
        float clusterCount, 
        float elementsPerCluster, 
        float clusterRadius, 
        float noiseScale, 
        float threshold, 
        GameObject[] elements,
        float elementTypeNoiseScale)
    {
        this.coord = coord;
        this.chunkSize = chunkSize;
        this.spacing = spacing;

        this.clusterCount = clusterCount;
        this.elementsPerCluster = elementsPerCluster;
        this.clusterRadius = clusterRadius;

        this.noiseScale = noiseScale;
        this.threshold = threshold;

        this.elements = elements;
        
        this.typeScale = elementTypeNoiseScale;

        parentObject = new GameObject($"Chunk_{coord.x}_{coord.y}");
        Generate();
    }
    void Generate()
    {
        for (int c = 0; c < clusterCount; c++)
        {
            
            Vector3 center = new Vector3(
                coord.x * chunkSize + Random.Range(0, chunkSize),
                Random.Range(-chunkHeight, chunkHeight),
                coord.y * chunkSize + Random.Range(0, chunkSize)
            );

            float noise = Perlin3D(
                center.x * noiseScale,
                center.y * noiseScale,
                center.z * noiseScale
            );

            GameObject selectedElement;

            if (noise < threshold)
                continue;

            float typeNoise = Perlin3D(
                center.x * typeScale,
                center.y * typeScale,
                center.z * typeScale
            );

            if (typeNoise < 0.33f)
                selectedElement = elements[0];
            else if (typeNoise < 0.44f)
                selectedElement = elements[1];
            else
                selectedElement = elements[2];


            for (int i = 0; i < elementsPerCluster; i++)
            {
                Vector3 offset = Random.insideUnitSphere * clusterRadius;

                float falloff = 1f - (offset.magnitude / clusterRadius);

                if (Random.value > falloff)
                    continue;

                Vector3 pos = center + offset;

                
                SpawnElement(pos.x, pos.y, pos.z, noise, selectedElement);
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

    void SpawnElement(float x, float y, float z, float noise, GameObject selectedElement)
    {
        Vector3 pos = new Vector3(x, y, z);

        GameObject element = Object.Instantiate(selectedElement, pos, Random.rotation);

        float size = Mathf.Lerp(2f, 8f, noise);

        element.transform.localScale = Vector3.one * size;
        element.transform.position = pos;
        element.transform.rotation = Random.rotation;

        element.transform.SetParent(parentObject.transform);
    }
    public void Destroy()
    {
        GameObject.Destroy(parentObject);
    }
}
