using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;
    public bool occupied;
    public string lane;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, string _lane)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        occupied = false;
        lane = _lane;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
