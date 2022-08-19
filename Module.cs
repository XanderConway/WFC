using System.Collections.Generic;
using UnityEngine;
public class Module
{
    // Module holds list of possible orientations, and valid sockets
    // Make valid neighbors one piece of data, have each rotation point to it. !!
    public HashSet<Module>[] validNeighbors = new HashSet<Module>[6];
    public GameObject tile;

    [SerializeField]
    public string tileName;
    public int orientation;
    public List<int>[] neighborIndicies = new List<int>[6];

    public Module()
    {
        
    }




}