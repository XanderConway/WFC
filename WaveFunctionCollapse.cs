using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    public Vector3Int dimensions;
    HashSet<PrototypeData>[,,] system;
       
    public void fillSystem(HashSet<PrototypeData> data) {
        for(int x = 0; x < dimensions.x; x++) {
            for(int y = 0; y < dimensions.y; y++) {
                for(int z = 0; z < dimensions.z; z++) {
                    system[x, y, z] = new HashSet<PrototypeData>(data);
                }
            }
        }
    }

    public bool finished() {
        for(int x = 0; x < dimensions.x; x++) {
            for(int y = 0; y < dimensions.y; y++) {
                for(int z = 0; z < dimensions.z; z++) {
                    if(system[x, y, z].Count > 1) {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public Vector3Int getMinEntropy() {
        float minEntropy = float.MaxValue;
        Vector3Int minCoords = new Vector3Int(-1, -1 ,-1);
        for(int x = 0; x < dimensions.x; x++) {
            for(int y = 0; y < dimensions.y; y++) {
                for(int z = 0; z < dimensions.z; z++) {
                    int count = system[x, y, z].Count;
                    if(count > 1 && count <= minEntropy) {
                        minEntropy = count;
                        minCoords = new Vector3Int(x, y, z);
                    }
                }
            }
        }

        return minCoords;
    }

    public void collapseCell(Vector3Int coords) {
        HashSet<PrototypeData> cell = system[coords.x, coords.y, coords.z];

        // list containing the data(s) with the highest probability
        List<PrototypeData> tiedData = new List<PrototypeData>();
        
        foreach(PrototypeData data in cell) {
            // Check likely probability, find min
            if(tiedData.Count == 0) {
                tiedData.Add(data);
            } else {
                if(tiedData[0].probaility < data.probaility) {
                    tiedData.Clear();
                    tiedData.Add(data);
                } else if(tiedData[0].probaility == data.probaility) {
                    tiedData.Add(data);
                }
            }
        }

        int index = Random.Range(0, tiedData.Count);
    
        PrototypeData choice = tiedData[index];

        print("Collapsed " + coords + " to " + choice.gameObject.name + " " + choice.orientation);
        cell.Clear();
        cell.Add(choice);

        //system[coords.x, coords.y, coords.z];
    }

    private Vector3Int getDirections(int i ) {
        switch(i) {
            case 0:
                return Vector3Int.right;
            case 1:
                return Vector3Int.forward;
            case 2:
                return Vector3Int.left;
            case 3:
                return Vector3Int.back;
            case 4:
                return Vector3Int.up;
            case 5:
                return Vector3Int.down;
            default:
                return Vector3Int.zero;
        }
    }

    private bool inBounds(Vector3Int coords) {
        bool x = coords.x >= 0 && coords.x < system.GetLength(0);
        bool y = coords.y >= 0 && coords.y < system.GetLength(1);
        bool z = coords.z >= 0 && coords.z < system.GetLength(2);

        return x && y && z;
    }

    public void propogate(Vector3Int coords) {
        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        stack.Push(coords);
        
        while(stack.Count > 0) {
            Vector3Int currentCoords = stack.Pop();
            HashSet<PrototypeData> current = system[currentCoords.x, currentCoords.y, currentCoords.z];

            for(int i = 0; i < 6; i++) {
                Vector3Int neighborCoords = getDirections(i) + currentCoords;
                if(!inBounds(neighborCoords)) {
                    continue;
                }

                HashSet<PrototypeData> neighborCell = system[neighborCoords.x, neighborCoords.y, neighborCoords.z];

                List<PrototypeData> invalids = new List<PrototypeData>();
                foreach(PrototypeData potentialNeighbor in neighborCell) {
                    // Is there any cell in current that would allow this neighbor?
                    bool valid = false;
                    foreach(PrototypeData potentialCurrent in current) {
                        if(potentialCurrent.validNeighbors[i].Contains(potentialNeighbor)) {
                            valid = true;
                            break;
                        }
                    }

                    // The neighbor cannot be this cell, remove it, then update neighbors
                    if(!valid) {
                        invalids.Add(potentialNeighbor);
                    }
                }

                // Remove invalids
                foreach(PrototypeData invalid in invalids) {
                    neighborCell.Remove(invalid);

                    if (!stack.Contains(neighborCoords))
                    {
                        stack.Push(neighborCoords);
                    }
                    //print("Removed " + invalid.gameObject.name + " " + invalid.orientation + " from " + neighborCoords);
                }
            }
        }

    }

    public void collapse() {
        while(!finished()) {
            Vector3Int coords = getMinEntropy();
            collapseCell(coords);
            propogate(coords);
        }
    }

    public void build(Vector3 startPos, float gridSize) {
        if(root != null) {
            Destroy(root);
        }
        root = new GameObject();

        for(int x = 0; x < dimensions.x; x++) {
            for(int y = 0; y < dimensions.y; y++) {
                for(int z = 0; z < dimensions.z; z++) {

                    // TODO create separate element for the choice
                    if(system[x, y, z].Count == 1) {
                        foreach(PrototypeData data in system[x, y, z]) {
                            Quaternion rot = Quaternion.AngleAxis(data.orientation * 90, data.gameObject.transform.up);
                            Vector3 pos = startPos + new Vector3(x, y, z) * gridSize;
                            GameObject tile = Instantiate(data.gameObject, pos, data.gameObject.transform.rotation * rot);

                            foreach(PrototypeData a in tile.GetComponents<PrototypeData>())
                            {
                                Destroy(a);
                            }

                            PrototypeData debug = tile.AddComponent<PrototypeData>();
                            debug.sockets = data.sockets;
                            debug.orientation = data.orientation;

                            tile.transform.parent = root.transform;
                        }
                    }
                }
            }
        }
    }

    private List<PrototypeData> data;
    private GameObject root;
    public void Start()
    {
        GeneratePrefabs genPrefab = GetComponent<GeneratePrefabs>();
        data = genPrefab.generatePrototypes();
        system = new HashSet<PrototypeData>[dimensions.x, dimensions.y, dimensions.z];
        fillSystem(new HashSet<PrototypeData>(data));
        collapse();
        // build(Vector3.zero, 2);
    }


    



    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space)) {
            fillSystem(new HashSet<PrototypeData>(data));
            collapse();
            build(Vector3.zero, 2);
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            //fillSystem(new HashSet<PrototypeData>(data));
            //collapse();
            //build(Vector3.zero, 2);
        }
    }
}
