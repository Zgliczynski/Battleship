using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissileScript : MonoBehaviour
{
    GameManager gameManager;
    EnemyScript enemyScript;
    public Vector3 targetTileLocaction;
    private int targetTile = -1;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyScript = GameObject.Find("Enemy").GetComponent<EnemyScript>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            gameManager.EnemyHitPlayer(targetTileLocaction, targetTile, collision.gameObject);
        }
        else
        {

        }
        Destroy(gameObject);
    }

    public void SetTarget(int target)
    {
        targetTile = target;
    }
}
