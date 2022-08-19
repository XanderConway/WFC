using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketComparator : IEqualityComparer<Socket>
{
    public bool Equals(Socket x, Socket y)
    {
        return x.id == y.id && x.flipped == y.flipped && x.orientation == y.orientation;
    }

    public int GetHashCode(Socket obj)
    {
        int x = obj.flipped ? 1 : 0;
        return obj.id * 100 + x;
    }
}

public class WfcTileData
{
    public List<Module> modules;
    //public List<List<int>[]> neighbors;

    private Dictionary<PrototypeData, int> dataToIndex;

    private Dictionary<Socket, List<int>>[] socketToNeighbors;

    public WfcTileData()
    {
        socketToNeighbors = new Dictionary<Socket, List<int>>[6];
    }


    public void addAllOrientations(List<PrototypeData> data)
    {
        for(int i = 0; i < 4; i++)
        {
            modules.Add(new Module());

        }
    }
}
