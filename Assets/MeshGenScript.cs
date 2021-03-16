using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenScript : MonoBehaviour
{
    public PhysicMaterial physicMaterial;

    public ChunkData chunkDatas;
    public MeshEditor editor;
    public Vector3 globalPosition;

    public Controller controllerScript;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
        globalPosition = gameObject.transform.position;
        chunkDatas = new ChunkData { globalPosition = globalPosition, currMesh = mesh, controllerScript = controllerScript };
        editor = new MeshEditor { chunkDatas = chunkDatas };
    }

    public void CreateMap()
    {
        foreach (Vector3 key in controllerScript.mapDict[globalPosition].Keys)
        {
            Block newBlock = new Block { localPosition = key, chunkDatas = chunkDatas, editor = editor, material = controllerScript.mapDict[globalPosition][key] };
            chunkDatas.blocks.Add(key, newBlock);
        }

        foreach (Block block in chunkDatas.blocks.Values)
        {
            block.Create();
        }
        editor.RenderMeshes();

        UpdateCollider();
    }

    public void UpdateCollider()
    {
        DestroyImmediate(this.GetComponent<MeshCollider>());
        var collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = this.chunkDatas.currMesh;
        collider.material = physicMaterial;
    }
}

public class ChunkData
{
    public Vector3 globalPosition;
    public Mesh currMesh;
    public Controller controllerScript;
    public Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();


    public int lastFaceIndex = 0;

    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<Vector2> newUvs = new List<Vector2>();
    public List<Vector3> newNormals = new List<Vector3>();
}


public class Face
{
    public int faceIndex;
    public FaceType faceDirection;
    public bool created;
}

public enum FaceType
{
    FrontFace = 0,
    BackFace = 1,
    TopFace = 2,
    BottomFace = 3,
    RightFace = 4,
    LeftFace = 5
};

public class MeshEditor
{
    public ChunkData chunkDatas;
    private int numMats = 2;

    private Vector2[] uvs = new Vector2[]{
            new Vector2(0, 0),
            new Vector2(0.33f, 0),
            new Vector2(0.66f, 0),
            new Vector2(1, 0),
            new Vector2(0, 0.5f),
            new Vector2(0.33f, 0.5f),
            new Vector2(0.66f, 0.5f),
            new Vector2(1, 0.5f),
            new Vector2(0, 1),
            new Vector2(0.33f, 1),
            new Vector2(0.66f, 1),
            new Vector2(1, 1)
    };
    private Vector3[] points = new Vector3[]
    {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
    };

    public void RenderMeshes()
    {
        chunkDatas.currMesh.vertices = chunkDatas.newVertices.ToArray();
        chunkDatas.currMesh.triangles = chunkDatas.newTriangles.ToArray();
        chunkDatas.currMesh.uv = chunkDatas.newUvs.ToArray();
        chunkDatas.currMesh.normals = chunkDatas.newNormals.ToArray();

        chunkDatas.currMesh.RecalculateBounds();
    }

    public void AddMeshes(Vector3 position, int[] verticesIndexs, int[] uvIndexs, int faceIndex, bool clockWises, Vector3 direction, int material)
    {
        for (int i = 0; i < 4; i++)
        {
            chunkDatas.newVertices.Add(points[verticesIndexs[i]] + position);
            chunkDatas.newUvs.Add(new Vector2(uvs[uvIndexs[i]].x, uvs[uvIndexs[i]].y / numMats + (1.0f / numMats * material)));
            chunkDatas.newNormals.Add(direction);
        }
        
        int triangleStart = faceIndex * 4;
        if (clockWises) chunkDatas.newTriangles = chunkDatas.newTriangles.Concat(new List<int> { triangleStart + 2, triangleStart + 1, triangleStart,
                                                               triangleStart + 1, triangleStart + 2, triangleStart + 3 }).ToList();
        else chunkDatas.newTriangles = chunkDatas.newTriangles.Concat(new List<int> { triangleStart, triangleStart + 1, triangleStart + 2,
                                                               triangleStart + 3, triangleStart + 2, triangleStart + 1 }).ToList();

    }

    public void DeleteMeshes(int faceIndex)
    {
        chunkDatas.newVertices.RemoveRange(faceIndex * 4, 4);
        chunkDatas.newTriangles.RemoveRange(faceIndex * 6, 6);
        for (int i = faceIndex * 6; i < chunkDatas.newTriangles.Count; i++)
        {
            chunkDatas.newTriangles[i] -= 4;
        }
        chunkDatas.newUvs.RemoveRange(faceIndex * 4, 4);
        chunkDatas.newNormals.RemoveRange(faceIndex * 4, 4);
    }
}


public class Block
{
    public ChunkData chunkDatas;
    public MeshEditor editor;
    public Vector3 localPosition;
    public int material;
    public List<Face> faces = new List<Face>();


    private int[][] verticesIndex = new int[][] { new int[] { 0, 1, 2, 5 }, new int[] { 3, 6, 4, 7 }, new int[] { 0, 3, 2, 4 },
                                                  new int[] { 1, 6, 5, 7 }, new int[] { 2, 5, 4, 7 }, new int[] { 0, 1, 3, 6 } };
    private int[][] uvIndex = new int[][] { new int[] { 0, 1, 4, 5 }, new int[] { 4, 5, 8, 9 }, new int[] { 1, 2, 5, 6 },
                                             new int[] { 5, 6, 9, 10 }, new int[] { 2, 3, 6, 7 }, new int[] { 6, 7, 10, 11 } };
    private bool[] clockwiseIndex = new bool[] { true, false, false, true, true, false };
    private Vector3[] normalIndex = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };
    private Vector3[] normalIndex2 = new Vector3[] { Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.up, Vector3.down};

    private Vector3[] nearFaces = new Vector3[] { new Vector3(16, 0, 0), new Vector3(-16, 0, 0), new Vector3(0, 0, 16), new Vector3(0, 0, -16) };


    public bool FaceCheckNear(int faceIndex)
    {
        Vector3 blockToCheck = localPosition + normalIndex2[faceIndex];
        Dictionary<Vector3, Dictionary<Vector3, int>> checkDict = chunkDatas.controllerScript.mapDict;


        if (checkDict[chunkDatas.globalPosition].ContainsKey(blockToCheck)) return true;

        Vector3 chunkPos = chunkDatas.controllerScript.getChunk(chunkDatas.globalPosition + blockToCheck);
        if (blockToCheck.x == 16) // check if on the edge of other chunks
        {
            Vector3 PosCheck = chunkDatas.globalPosition + nearFaces[0];
            if (checkDict.ContainsKey(PosCheck))
            {
                if (checkDict[PosCheck].ContainsKey(blockToCheck - nearFaces[0])) return true;
            }
        }
        if (blockToCheck.x == -1) // check if on the edge of other chunks
        {
            Vector3 PosCheck = chunkDatas.globalPosition + nearFaces[1];
            if (checkDict.ContainsKey(PosCheck))
            {
                if (checkDict[PosCheck].ContainsKey(blockToCheck - nearFaces[1])) return true;
            }
        }
        if (blockToCheck.z == 16) // check if on the edge of other chunks
        {
            Vector3 PosCheck = chunkDatas.globalPosition + nearFaces[2];
            if (checkDict.ContainsKey(PosCheck))
            {
                if (checkDict[PosCheck].ContainsKey(blockToCheck - nearFaces[2])) return true;
            }
        }
        if (blockToCheck.z == -1) // check if on the edge of other chunks
        {
            Vector3 PosCheck = chunkDatas.globalPosition + nearFaces[3];
            if (checkDict.ContainsKey(PosCheck))
            {
                if (checkDict[PosCheck].ContainsKey(blockToCheck - nearFaces[3])) return true;
            }
        }
        return false;
    }

    public void Create() // this generate/add mesh to the chunk's entire mesh(not yet rendered), editor.RenderMeshes renders it.
    {
        if (faces.Count != 0)
        {
            Delete();
            faces.Clear();
        }

        List<FaceType> allFaces = FaceType.GetValues(typeof(FaceType)).Cast<FaceType>().ToList();
        for (int i=0; i<6; i++)
        {
            if (!FaceCheckNear(i))
            {
                faces.Add(new Face { faceDirection = allFaces[i], faceIndex = chunkDatas.lastFaceIndex, created = false });
                chunkDatas.lastFaceIndex++;
            }
        }

        foreach (Face face in faces)
        {
            if (face.created == false)
            {
                face.created = true;
                editor.AddMeshes(localPosition, verticesIndex[(int)face.faceDirection], uvIndex[(int)face.faceDirection],
                                               face.faceIndex, clockwiseIndex[(int)face.faceDirection], normalIndex[(int)face.faceDirection], material);
            }
        }
    }

    public void SubtractFaceIndex(int faceIndex, int numIndex)
    {
        foreach (Face face in faces)
        {
            if (face.faceIndex > faceIndex)
            {
                face.faceIndex = face.faceIndex - numIndex;

            }
            else if (face.faceIndex == faceIndex)
            {
                Debug.LogError("WTF THIS SHOULDN'T HAPPEN");
            }
        }
    }

    public void Delete()
    {
        int numFacesDeletion = 0;
        int maxFaceIndex = 0;
        foreach (Face face in faces)
        {
            if (face.faceIndex > maxFaceIndex)
            {
                maxFaceIndex = face.faceIndex;
            }
        }

        foreach (Face face in faces)
        {
            if (!face.created)
            {
                Debug.LogError("Face Uncreated Before Deletion");
            }
            editor.DeleteMeshes(face.faceIndex - numFacesDeletion);
            numFacesDeletion++;
        }

        foreach (Block currBlock in chunkDatas.blocks.Values)
        {
            if (currBlock != this)
            {
                currBlock.SubtractFaceIndex(maxFaceIndex, numFacesDeletion);
            }
        }
        chunkDatas.lastFaceIndex = chunkDatas.lastFaceIndex - numFacesDeletion;
    }
}
    //public void CheckFace()



