using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeData : MonoBehaviour
{
    private string[] faceNames ={"X", "Z", "-X", "-Z", "Top", "Botton"};

    // X, Y, Z
    public Socket[] sockets = new Socket[6];
    public List<string> excludedIds;
    public HashSet<PrototypeData>[] validNeighbors = new HashSet<PrototypeData>[6];

    public int orientation;

    public int probaility = 1;

    void Awake() {
        for(int i = 0; i < 6; i++) {
            validNeighbors[i] = new HashSet<PrototypeData>();

        }
    }


    public override string ToString()
    {
        string result = "";
        //for(int i =0; i < 6; i++) {
        //    for(int j = 0; j < validNeighbors[i].Count; j++)
        //    {
        //        result += (validNeighbors[i][j].model.name) + "\n";
        //    }
        //    //result += faceNames[i] + ": " + sockets[i].id + "\n";
        //}

        return result;

    }
}
