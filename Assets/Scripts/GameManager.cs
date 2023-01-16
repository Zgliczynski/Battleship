using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool setupComplete = false;
    private bool playerTurn = true;

    private ShipScript shipScript;
    
    private int shipsIndex = 0;
    private int enemyShipsCount = 5;
    private int playerShipsCount = 5;

    [Header("List")]
    private List<int[]> enemyShips;
    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();
    public List<TileScript> allTileScript;


    [Header("Ships")]
    public GameObject[] ships;
    
    [Header("Enemy")]
    public EnemyScript enemyScript;

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Text topText;
    public Text playerText;
    public Text enemyText;

    [Header("Objects")]
    public GameObject missilePrefabs;
    public GameObject enemyMissilePrefabs;
    public GameObject woodDock;
    public GameObject firePrefabs;

    private void Start()
    {
        shipScript = ships[shipsIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        replayBtn.onClick.AddListener(() => ReplayCLicked());
        enemyShips = enemyScript.PlaceEnemyShips();
    }

    private void NextShipClicked()
    {
        if(!shipScript.OnGameBoard())
        {
            shipScript.FlashColor(Color.red);
        }
        else
        {
            if(shipsIndex <= ships.Length - 2)
            {
                shipsIndex++;
                shipScript = ships[shipsIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                topText.text = "Guess an enemy tile";
                setupComplete = true;

                for (int i = 0; i < ships.Length; i++)
                    ships[i].SetActive(false);
            }
        }
    }

    public void TileClicked(GameObject tile)
    {
        if(setupComplete && playerTurn)
        {
            Vector3 tilePos = tile.transform.position;
            tilePos.y += 15;
            playerTurn = false;
            Instantiate(missilePrefabs, tilePos, missilePrefabs.transform.rotation);
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
                    enemyFires.Add(Instantiate(firePrefabs, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                else
                {
                    topText.text = "HIT!!!";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                break;
            }
        }
        if(hitCount == 0)
        {
            topText.text = "MISSED!!!";
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
        }
        Invoke("EndPlayerTurn", 1.0f);
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
        Invoke("EndEnemyTurn", 2.0f);
    }

    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
        foreach (GameObject fire in playerFires) fire.SetActive(true);
        foreach (GameObject fire in enemyFires) fire.SetActive(false);
        enemyText.text = enemyShipsCount.ToString();
        topText.text = "Enemy's turn";
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if (playerShipsCount < 1)
            GameOver("YOU LOSE!!!");
    }

    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
        foreach (GameObject fire in playerFires) fire.SetActive(false);
        foreach (GameObject fire in enemyFires) fire.SetActive(true);
        playerText.text = playerShipsCount.ToString();
        topText.text = "Select a tile";
        playerTurn = true;
        ColorAllTiles(1);
        if (enemyShipsCount < 1)
            GameOver("YOU WIN!!!");
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScript tileScript in allTileScript)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    void GameOver(string winner)
    {
        topText.text = "Game Over : " + winner;
        replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }

    void ReplayCLicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
