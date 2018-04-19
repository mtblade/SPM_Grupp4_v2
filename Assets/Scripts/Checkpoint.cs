using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public PlayerController Player;
    private Vector3 respawnPoint;

    private void Awake()
    {
        respawnPoint = (Vector3)this.gameObject.transform.GetChild(0).position;
        if (Player != null)
            Player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Player.GetComponent<DeathHandler>().SetRespawnPosition(respawnPoint);
        }
    }
}
