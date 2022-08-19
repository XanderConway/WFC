using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Object Import settings
// Scaling: FBX All
// -Z Forward
// Y Up
// Apply Transform
public class GeneratePrefabs : MonoBehaviour
{

    public List<GameObject> tiles;
    public float cubeHalfSize = 1;
    public int scaleFactor = 100;

    private SocketGenerator socketGenerator;

    private static float prefabOffet = 0;
    public List<PrototypeData> generatePrototypes()
    {
        socketGenerator = new SocketGenerator();
        GameObject root = new GameObject("Prototypes");

        List<PrototypeData> prototypes = new List<PrototypeData>();
        foreach (GameObject tile in tiles)
        {
            foreach (Transform childTransform in tile.transform)
            {
                GameObject child = childTransform.gameObject;

                GameObject childCopy = copyObject(child, root);
                List<PrototypeData> data = makePrototype(childCopy);

                prototypes.AddRange(data);
                
            }
        }

        setValidNeighbors(prototypes);

        foreach(PrototypeData p in prototypes)
        {
           // print(p);
        }
        return prototypes;
    }

    List<PrototypeData> makePrototype(GameObject prototype)
    {

        //print(prototype.name);
        Mesh mesh = prototype.GetComponent<MeshFilter>().mesh;

        // Create lists of the edge vertices

        // 0,1 - x 2,3 y 4,5 z
        HashSet<Vector2Int>[] faces = new HashSet<Vector2Int>[6];
        for(int i = 0; i < 6; i++) {
            faces[i] = new HashSet<Vector2Int>();
        }


        // Vertex positions depend on the origin of the object
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            // xPos face
            Vector3Int vert = roundVector(mesh.vertices[i], scaleFactor);
            // print(vert.x);

            if (vert.x == cubeHalfSize * scaleFactor)
            {
                faces[0].Add(new Vector2Int(vert.z, vert.y));
            }
            else if (vert.x == -cubeHalfSize * scaleFactor)
            {
                faces[2].Add(new Vector2Int(-vert.z, vert.y));
            }

            // side face
            if (vert.z == cubeHalfSize * scaleFactor)
            {
                faces[1].Add(new Vector2Int(-vert.x, vert.y));
            }
            else if (vert.z == -cubeHalfSize * scaleFactor)
            {
                faces[3].Add(new Vector2Int(vert.x, vert.y));
            }

            // Top and bottom (Blender coordinates still apply)
            if (vert.y == cubeHalfSize * scaleFactor * 2)
            {
                faces[4].Add(new Vector2Int(vert.x, vert.y));
            }
            else if (vert.y == 0)
            {
                faces[5].Add(new Vector2Int(vert.x, vert.y));
            }
        }



        // Add socket names to prototype data
        //PrototypeData data = new PrototypeData();
        // PrototypeData data = prototype.AddComponent<PrototypeData>();

        List<PrototypeData> prototypes = new List<PrototypeData>();
        for(int orientation = 0; orientation < 4; orientation++)
        {

            PrototypeData data = prototype.AddComponent<PrototypeData>();
            data.orientation = orientation;
            for (int i = 0; i < 4; i++)
            {
                int rotatedIndex = (i + orientation) % 4;
                //faces[i].Sort(new Vector2IntComparator());
                data.sockets[i] = socketGenerator.getSocket(faces[rotatedIndex]);
            }

            for (int i = 4; i < 6; i++)
            {
                //faces[i].Sort(new Vector2IntComparator());
                data.sockets[i] = socketGenerator.getSocketVertical(faces[i], orientation);
            }

            prototypes.Add(data);
        }
        return prototypes;
    }

    // Create mapping of socket to valid neighbor, neighbor needs an orientation
    // use mappings to create list of valid neighbors separately, modules should have their own neighbor lists, unless you want to fuck around with rotations


    public void setValidNeighbors(List<PrototypeData> prototypes)
    {
        for(int i = 0; i < prototypes.Count; i++)
        {
            for(int j = 0; j < prototypes.Count; j++)
            {
                // Lateral sockets
                for(int face = 0; face < 4; face++)
                {
                    // 0-1, 2-3
                    int oppositeFace = (face + 2) % 4; 
                    Socket a = prototypes[i].sockets[face];
                    Socket b = prototypes[j].sockets[oppositeFace];

                    if(validSockets(a, b)) {
                        prototypes[i].validNeighbors[face].Add(prototypes[j]);
                        //prototypes[i].neighborNames[face] += (prototypes[j].gameObject.name + " " + prototypes[j].orientation + "\n");
                       print("Added " + prototypes[j].gameObject.name + prototypes[j].orientation + "to " + prototypes[i].gameObject.name + prototypes[i].orientation + " " + face);
                    }
                }

                //Vertical sockets

                if(validSockets(prototypes[i].sockets[4], prototypes[j].sockets[5]) && prototypes[i].orientation == prototypes[j].orientation) {
                    prototypes[i].validNeighbors[4].Add(prototypes[j]);
                }

                if (validSockets(prototypes[i].sockets[5], prototypes[j].sockets[4]) && prototypes[i].orientation == prototypes[j].orientation) {
                    prototypes[i].validNeighbors[5].Add(prototypes[j]);
                }
            }
        }
    }

    private GameObject copyObject(GameObject tile, GameObject parent) {
        GameObject prototype = Instantiate(tile);
        prototype.transform.position = parent.transform.position;
        prototype.transform.position += Vector3.right * prefabOffet;
        prototype.transform.parent = parent.transform;
        prefabOffet += tile.transform.lossyScale.x;

        return prototype;
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
        //print(temp.SetEquals(temp2));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
