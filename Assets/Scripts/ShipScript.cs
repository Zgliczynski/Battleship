using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    List<GameObject> touchTiles = new List<GameObject>();
    public float xOffset = 0;
    public float zOffset = 0;

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    public Vector3 GetOffsetVec(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + xOffset, 2, tilePos.z + zOffset);
    }
}
