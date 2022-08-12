using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePrefabs : MonoBehaviour
{

    public List<GameObject> tiles;
    public float cubeHalfSize = 1;
    public int scaleFactor = 100;

    private SocketGenerator socketGenerator;

    private static float prefabOffet = 0;
    private void generatePrototypes()
    {

        GameObject root = new GameObject("Prototypes");

        List<PrototypeData> prototypes = new List<PrototypeData>();
        foreach (GameObject tile in tiles)
        {
            foreach (Transform childTransform in tile.transform)
            {
                GameObject child = childTransform.gameObject;
                PrototypeData data = makePrototype(child, root);
                prototypes.Add(data);
            }
        }

        setValidNeighbors(prototypes);

        foreach(PrototypeData p in prototypes)
        {
            print(p);
        }
    }


    PrototypeData makePrototype(GameObject tile, GameObject prototypes)
    {

        print(tile.name);
        Mesh mesh = tile.GetComponent<MeshFilter>().mesh;

        // Create lists of the edge vertices

        // 0,1 - x 2,3 y 4,5 z
        List<Vector2Int>[] faces = new List<Vector2Int>[6];
        for(int i = 0; i < 6; i++) {
            faces[i] = new List<Vector2Int>();
        }


        // Vertex positions depend on the origin of the object
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            // xPos face
            Vector3Int vert = roundVector(mesh.vertices[i], scaleFactor);
            // print(vert.x);

            if (vert.x == cubeHalfSize * scaleFactor)
            {
                faces[0].Add(new Vector2Int(vert.y, vert.z));
            }
            else if (vert.x == -cubeHalfSize * scaleFactor)
            {
                faces[2].Add(new Vector2Int(vert.y, vert.z));
            }

            // side face
            if (vert.y == cubeHalfSize * scaleFactor)
            {
                faces[1].Add(new Vector2Int(vert.x, vert.z));
            }
            else if (vert.y == -cubeHalfSize * scaleFactor)
            {
                faces[3].Add(new Vector2Int(vert.x, vert.z));
            }

            // Top and bottom (Blender coordinates still apply)
            if (vert.z == cubeHalfSize * scaleFactor)
            {
                faces[4].Add(new Vector2Int(vert.x, vert.y));
            }
            else if (vert.z == -cubeHalfSize * scaleFactor)
            {
                faces[5].Add(new Vector2Int(vert.x, vert.y));
            }
        }

        // Instantiate the prototype
        GameObject prototype = Instantiate(tile);
        prototype.transform.position = prototypes.transform.position;
        prototype.transform.position += Vector3.right * prefabOffet;
        prototype.transform.parent = prototypes.transform;
        prefabOffet += tile.transform.lossyScale.x;

        // Add socket names to prototype data
        PrototypeData data = new PrototypeData();
        data.model = tile;
        for (int i = 0; i < 4; i++) {
            faces[i].Sort(new Vector2IntComparator());
            data.sockets[i] = socketGenerator.getSocket(faces[i]);
        }

        for(int i = 4; i < 6; i++)
        {
            faces[i].Sort(new Vector2IntComparator());
            data.sockets[i] = socketGenerator.getSocketVertical(faces[i], 0);
        }

        // Add data to object
        PrototypeMono prototypeMono = prototype.AddComponent<PrototypeMono>();
        prototypeMono.data = data;

        return data;
    }

    public void setValidNeighbors(List<PrototypeData> prototypes)
    {
        for(int i = 0; i < prototypes.Count; i++)
        {
            for(int j = 0; j < prototypes.Count; j++)
            {
                // Lateral sockets
                for(int face = 0; face < 4; face++)
                {
                    // 0-1, 
                    int oppositeFace = (face + 2) % 4; 
                    if(validSockets(prototypes[i].sockets[face], prototypes[j].sockets[oppositeFace])) {
                        prototypes[i].validNeighbors[face].Add(prototypes[j]);
                        // print("Added " + prototypes[j].gameObject.name);
                    }
                }

                //Vertical sockets

                if(validSockets(prototypes[i].sockets[4], prototypes[j].sockets[5])) {
                    prototypes[i].validNeighbors[4].Add(prototypes[j]);
                }

                if (validSockets(prototypes[i].sockets[5], prototypes[j].sockets[4])) {
                    prototypes[i].validNeighbors[5].Add(prototypes[j]);
                }
            }
        }
    }

    private bool validSockets(Socket a, Socket b)
    {

        if(a.vertical)
        {
            return a.id == b.id;
        }

        if(a.symmetric)
        {
            return a.id == b.id;
        } else
        {
            return a.id == b.id && a.flipped != b.flipped;
        }
    }

    Vector3Int roundVector(Vector3 vec, int scale)
    {
        int x = (int)Mathf.Round(vec.x * scale);
        int y = (int)Mathf.Round(vec.y * scale);
        int z = (int)Mathf.Round(vec.z * scale);

        return new Vector3Int(x, y, z);
    }

    // Start is called before the first frame update
    void Start()
    {
        socketGenerator = new SocketGenerator();
        generatePrototypes();

        //print(temp.SetEquals(temp2));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
