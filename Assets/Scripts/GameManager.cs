using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool setupComplete = false;
    private bool playerTurn = true;

    public GameObject[] ships;
    private int shipsIndex = 0;
    private ShipScript shipScript;

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;

    private void Start()
    {
        shipScript = ships[shipsIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
    }

    private void NextShipClicked()
    {
        if(shipsIndex <= ships.Length - 2)
        {
            shipsIndex++;
            shipScript = ships[shipsIndex].GetComponent<ShipScript>();
            // shipScript.FlashColor(Color.yellow);
        }
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
            shipScript.SetClickedTile(tile);
        }
    }

    private void PlaceShips(GameObject tile)
    {
        shipScript = ships[shipsIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipsIndex].transform.localPosition = newVec;
    }

    private void RotateClicked()
    {
        shipScript.RotateShips();
    }
}
