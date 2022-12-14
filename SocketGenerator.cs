using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class VectorListComparator : IEqualityComparer<List<Vector2Int>>
{


    public bool Equals(List<Vector2Int> x, List<Vector2Int> y)
    {
        if (x.Count != y.Count)
        {
            return false;
        }

        for (int i = 0; i < x.Count; i++)
        {
            if (x[i].x != y[i].x || x[i].y != y[i].y)
            {
                return false;
            }
        }

        return true;
    }


    public int GetHashCode(List<Vector2Int> obj)
    {
        int sum = 0;

        for (int i = 0; i < obj.Count; i++)
        {
            sum += obj[i].x;
            sum += obj[i].y * 2;
        }
        return sum;
    }
}


class HashsetComparator : IEqualityComparer<HashSet<Vector2Int>>
{


    public bool Equals(HashSet<Vector2Int> x, HashSet<Vector2Int> y)
    {
        return x.SetEquals(y);
    }


    public int GetHashCode(HashSet<Vector2Int> obj)
    {
        int sum = 0;
        foreach(Vector2Int val in obj)
        {
            sum += val.x + val.y;
        }
        return sum;
    }
}

public class Vector2IntComparator : IComparer<Vector2Int>
{
    int IComparer<Vector2Int>.Compare(Vector2Int x, Vector2Int y)
    {
        return (x.x + x.y - y.x - y.y);
    }
}

public class SocketGenerator
{
    private int id_num;

    private Dictionary<HashSet<Vector2Int>, Socket> lateralSockets;
    private Dictionary<HashSet<Vector2Int>, Socket> verticalSockets;

    public SocketGenerator()
    {
        lateralSockets = new Dictionary<HashSet<Vector2Int>, Socket>(new HashsetComparator());
        verticalSockets = new Dictionary<HashSet<Vector2Int>, Socket>(new HashsetComparator());
    }

    public Socket getSocket(HashSet<Vector2Int> verts)
    {
        Socket socket = null;
        lateralSockets.TryGetValue(verts, out socket);

        if (socket != null)
        {
            return socket;
        }
        else
        {
            HashSet<Vector2Int> flipped = getSymmetric(verts);
            lateralSockets.TryGetValue(flipped, out socket);

            if (socket != null)
            {
                Socket flipped_socket =  new Socket(socket.id, false, -1, true, false);
                lateralSockets.Add(verts, flipped_socket);
                return flipped_socket;
            }
            else
            {
                bool sym = new HashsetComparator().Equals(flipped, verts);
                Socket newSocket = createNewSocket(sym, -1, false);
                lateralSockets.Add(verts, newSocket);
                return newSocket;
            }
        }
    }

    public Socket getSocketVertical(HashSet<Vector2Int> verts, int orientation)
    {
        Socket socket = null;
        verticalSockets.TryGetValue(verts, out socket);

        if (socket != null)
        {
            return socket;
        }
        else
        {
            Socket newSocket = createNewSocket(false, orientation, false, true);
            verticalSockets.Add(verts, newSocket);
            return newSocket;
        }
    }

    private Socket createNewSocket(bool sym, int orientation, bool flipped, bool vertical = false)
    {
        id_num++;
        return new Socket(id_num, sym, orientation, flipped, vertical);
    }

    private HashSet<Vector2Int> getSymmetric(HashSet<Vector2Int> verts)
    {

        HashSet<Vector2Int> flipped = new HashSet<Vector2Int>();

        
        foreach(Vector2Int vert in verts)
        {
            Vector2Int reflected = new Vector2Int(-vert.x, vert.y);
            flipped.Add(reflected);
        }
        return flipped;
    }
}
