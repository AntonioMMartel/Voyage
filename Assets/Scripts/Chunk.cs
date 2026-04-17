using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk
{
    public Vector2Int coord;
    public GameObject parentObject;


    private float chunkSize, spacing, clusterCount, elementsPerCluster, clusterRadius, noiseScale, threshold, typeScale;
    private bool usesEffect;
    private GameObject[] elements, elementsGeneric, elementsTerrain;

    private Vector3 playerSpawn = new Vector3(0, 210, 0);
    private float spawnSafeRadius = 20f;
    public float chunkHeight = 700f;


    public Chunk(Vector2Int coord,
        float chunkSize, 
        float spacing, 
        float clusterCount, 
        float elementsPerCluster, 
        float clusterRadius, 
        float noiseScale, 
        float threshold, 
        GameObject[] elements,
        float elementTypeNoiseScale,
        bool usesEffect,
        GameObject[] elementsGeneric,
        GameObject[] elementsTerrain)
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
        this.usesEffect = usesEffect;

        this.elementsTerrain = elementsTerrain;

        this.elementsGeneric = elementsGeneric;

        parentObject = new GameObject($"Chunk_{coord.x}_{coord.y}");

        GeneratePerlin();
        GenerateGeneric();
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        float genericElementsPerChunk = 1;

        for (int i = 0; i < genericElementsPerChunk; i++)
        {
            float randomX = Random.Range(0, chunkSize);
            float randomZ = Random.Range(0, chunkSize);

            Vector3 pos = new Vector3(
                coord.x * chunkSize + randomX,
                0,
                coord.y * chunkSize + randomZ
            );


            float size = Random.Range(5f, 7f);
            int selectedElement = GetWeightedIndex();

            Quaternion rotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0);
            if ( selectedElement > 2 ) // Spagetti porque tengo que terminar y me ha pasado que importe mal los assets
            {
                rotation = Quaternion.Euler(-90f, Random.Range(0f, 360f), 0);
            }
                
            SpawnElement(pos.x, pos.y, pos.z, 0, elementsTerrain[selectedElement], size, rotation);
        }
    }

    int GetWeightedIndex() // Menos pirámides más arcos
    {
        float[] weights = new float[] { 3f, 3f, 3f, 1f, 1f, 1f };
        float totalWeight = 0f;

        for (int i = 0; i < weights.Length; i++)
            totalWeight += weights[i];

        float randomValue = Random.Range(0, totalWeight);

        float cumulative = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];

            if (randomValue <= cumulative)
                return i;
        }

        return 0;
    }

    void GenerateGeneric()
    {
        float genericDensity = Mathf.FloorToInt((chunkSize * chunkSize) / spacing * spacing);
        float genericElementsPerChunk = elementsPerCluster/15;
        for (int i = 0; i < genericElementsPerChunk; i++)
        {
            float randomX = Random.Range(0, chunkSize);
            float randomY = Random.Range(100, chunkHeight);
            float randomZ = Random.Range(0, chunkSize);

            Vector3 pos = new Vector3(
                coord.x * chunkSize + randomX,
                randomY,
                coord.y * chunkSize + randomZ
            );


            float size = Random.Range(2f, 12f);

            SpawnElement(pos.x, pos.y, pos.z, 0, elementsGeneric[Random.Range(0, elementsGeneric.Length)], size, Random.rotation);
        }
    }
    void GeneratePerlin()
    {
        for (int c = 0; c < clusterCount; c++)
        {
            
            Vector3 center = new Vector3(
                coord.x * chunkSize + Random.Range(0, chunkSize),
                Random.Range(100, chunkHeight),
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

                
                SpawnElement(pos.x, pos.y, pos.z, noise, selectedElement, 0, Random.rotation);
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

    void SpawnElement(float x, float y, float z, float noise, GameObject selectedElement, float size, Quaternion rotation)
    {
        Vector3 pos = new Vector3(x, y, z);

        if (Vector3.Distance(pos, playerSpawn) > spawnSafeRadius) //Prevenimos spawn sobre jugador al spawnear
        {
            GameObject element = Object.Instantiate(selectedElement, pos, rotation);


            if (noise == 0 && size == 0)
            {
                size = Random.Range(2f, 4f);
            }
            else if (size == 0)
            {
                size = Mathf.Lerp(2f, 4f, noise);
            }

            if (usesEffect)
            {
                element.transform.localScale = Vector3.zero;
                var effect = element.AddComponent<ScaleInEffect>();
                effect.Play(size);
            }
            else
            {
                element.transform.localScale = Vector3.one * size;
            }

            element.transform.SetParent(parentObject.transform);
        }
    }
    public void Destroy()
    {
        GameObject.Destroy(parentObject);
    }
}
