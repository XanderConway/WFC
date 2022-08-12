using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrototypeData
{
    private string[] faceNames ={"X", "Z", "-X", "-Z", "Top", "Botton"};

    // X, Y, Z
    public Socket[] sockets = new Socket[6];
    public List<string> excludedIds;
    public List<PrototypeData>[] validNeighbors = new List<PrototypeData>[6];
    public int orientation;
    public GameObject model;

    void Start() {
        for(int i = 0; i < 6; i++) {
            validNeighbors[i] = new List<PrototypeData>();
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
