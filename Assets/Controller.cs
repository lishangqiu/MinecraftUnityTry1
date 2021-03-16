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
        noise.SetNoiseType(gameSettings.noiseType);


        for (int x = -gameSettings.minRenderDistance; x < gameSettings.minRenderDistance; x++)
        {
            for (int y = -gameSettings.minRenderDistance; y < gameSettings.minRenderDistance; y++)
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
        for (int x = 0; x < gameSettings.length; x++)
        {
            for (int z = 0; z < gameSettings.width; z++)
            {
                UpdateMap(new Vector3(x, 2, z), 1, chunkPosition);
                UpdateMap(new Vector3(x, 1, z), 1, chunkPosition);
                UpdateMap(new Vector3(x, 0, z), 1, chunkPosition);

                float noiseGen = noise.GetNoise((x + chunkPosition.x) * gameSettings.noiseConcentration, (z + chunkPosition.z) * gameSettings.noiseConcentration) + 1;
                int height = (int)(noiseGen * gameSettings.noiseMultiplication - gameSettings.noiseMultiplication + 2) + 2;
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

