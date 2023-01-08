using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool setupComplete = false;
    private bool playerTurn = true;

    public GameObject[] ships;
    private int shipsIndex = 0;
    private ShipScript shipScript;

    private void Start()
    {
        shipScript = ships[shipsIndex].GetComponent<ShipScript>();
    }
    public void TileClicked(GameObject tile)
    {
        if(setupComplete && playerTurn)
        {
            // drop missile
        }
        else if (!setupComplete)
        {
            PlaceShips(tile);
        }
    }

    private void PlaceShips(GameObject tile)
    {
        shipScript = ships[shipsIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipsIndex].transform.localPosition = newVec;
    }
}
