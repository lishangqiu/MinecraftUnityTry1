using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject parentChunk;
    public static Dictionary <Vector3, GameObject> Chunks = new Dictionary<Vector3, GameObject>();
    public Dictionary <Vector3, Dictionary <Vector3, int>> mapDict = new Dictionary<Vector3, Dictionary <Vector3, int>>();

    private FastNoiseLite noise;
    // Start is called before the first frame update
    void Start()
    {
        noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);


        for (int x = -chunkConstants.minRenderDistance; x < chunkConstants.minRenderDistance; x++)
        {
            for (int y = -chunkConstants.minRenderDistance; y < chunkConstants.minRenderDistance; y++)
            {
                Vector3 chunkPos = new Vector3(x * 16, 0, y * 16);
                GameObject newChunk = Instantiate(parentChunk, chunkPos, transform.rotation);

                MeshGenScript chunkScript = newChunk.GetComponent<MeshGenScript>();
                chunkScript.Init();
                DefaultChunkMap(chunkPos);
                //chunkScript.CreateMap(mapDict[chunkPos]);
                
                Chunks.Add(chunkPos, newChunk);
                //Debug.Log(chunkPos);
            }
        }

        foreach (GameObject chunk in Chunks.Values)
        {
            chunk.GetComponent<MeshGenScript>().CreateMap();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateMap(Vector3 Pos, int Material, Vector3 chunkPosition)
    {
        if (mapDict[chunkPosition].ContainsKey(Pos))
        {
            mapDict[chunkPosition][Pos] = Material;
        }
        else
        {
            mapDict[chunkPosition].Add(Pos, Material);
        }
    }

    public void DefaultChunkMap(Vector3 chunkPosition)
    {
        mapDict.Add(chunkPosition, new Dictionary<Vector3, int>());
        for (int x = 0; x < chunkConstants.length; x++)
        {
            for (int z = 0; z < chunkConstants.width; z++)
            {
                UpdateMap(new Vector3(x, 2, z), 1, chunkPosition);
                UpdateMap(new Vector3(x, 1, z), 1, chunkPosition);
                UpdateMap(new Vector3(x, 0, z), 1, chunkPosition);

                float noiseGen = noise.GetNoise((x + chunkPosition.x) * chunkConstants.noiseConcentration, (z + chunkPosition.z) * chunkConstants.noiseConcentration) + 1;
                int height = (int)(noiseGen * chunkConstants.noiseMultiplication - chunkConstants.noiseMultiplication + 2) + 2;
                //Debug.Log(noiseGen);
                for (int y = 0; y < height; y++)
                {
                    UpdateMap(new Vector3(x, 3+y, z), 0, chunkPosition);
                }
            }
        }
    }

    public Vector3 getChunk(Vector3 blockPos)
    {
        int newX;
        int newZ;

        if (blockPos.x < 0) newX = (int) ((blockPos.x / 16) - 1);
        else newX = (int) blockPos.x / 16;

        if (blockPos.z < 0) newZ = (int) ((blockPos.z / 16) - 1);
        else newZ = (int) blockPos.z / 16;

        return new Vector3(newX, 0, newZ) * 16;
    }
}

public static class chunkConstants
{
    public const int length = 16;
    public const int width = 16;
    public const int height = 1;

    public const int minRenderDistance = 2; // It is the actual distance divided by 2
    public const int maxRenderDistance = 8;

    // Noise Settings
    public const float noiseConcentration = 4;
    public const float noiseMultiplication = 3;
}