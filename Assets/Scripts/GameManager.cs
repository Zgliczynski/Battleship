using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool setupComplete = false;
    private bool playerTurn = true;

    public GameObject[] ships;
    private int shipsIndex = 0;
    private ShipScript shipScript;

    private List<int[]> enemyShips;
    private List<GameObject> playerFires;

    private int enemyShipsCount = 5;
    private int playerShipsCount = 5;

    [Header("Enemy")]
    public EnemyScript enemyScript;

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Text topText;
    public Text playerText;
    public Text enemyText;

    [Header("Objects")]
    public GameObject missilePrefabs;
    public GameObject enemyMissilePrefabs;
    public GameObject woodDeck;
    public GameObject firePrefabs;

    private void Start()
    {
        shipScript = ships[shipsIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        enemyShips = enemyScript.PlaceEnemyShips();
    }

    private void NextShipClicked()
    {
        if(shipsIndex <= ships.Length - 2)
        {
            shipsIndex++;
            shipScript = ships[shipsIndex].GetComponent<ShipScript>();
            // shipScript.FlashColor(Color.yellow);
        }
        else
        {
            enemyScript.PlaceEnemyShips();
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

    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach(int[] tileNumArrey in enemyShips)
        {
            if (tileNumArrey.Contains(tileNum))
            {
                for (int i = 0; i < tileNumArrey.Length; i++)
                {
                    if (tileNumArrey[i] == tileNum)
                    {
                        tileNumArrey[i] = -5;
                        hitCount++;
                    }
                    else if (tileNumArrey[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == tileNumArrey.Length)
                {
                    enemyShipsCount--;
                    topText.text = "SUNK!!!!";
                    //enemyFires
                    //color
                }
                else
                {
                    topText.text = "HIT!!!";
                    //color
                }
                break;
            }
        }
        if(hitCount == 0)
        {
            topText.text = "MISSED!!!";
            //color
        }
        //Invoke("EndPlayerTurn", 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefabs, tile, Quaternion.identity));
        if (hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            playerShipsCount--;
            playerText.text = playerShipsCount.ToString();
            enemyScript.SunkPlayer();
        }
        //Invoke("EndEnemyTurn", 2.0f);
    }
}
