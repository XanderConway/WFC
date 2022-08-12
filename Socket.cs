using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Socket
{
    public int id;
    public bool symmetric;
    public bool flipped;
    public bool vertical;

    public Socket(int id, bool sym, int orientation, bool flipped, bool vertical) {
        this.id = id;
        this.symmetric = sym;
        this.flipped = flipped;
        this.vertical = vertical;
    }
}

