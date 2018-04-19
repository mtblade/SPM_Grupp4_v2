using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) {
            Debug.Log("Player hit DeathZone!");
            col.gameObject.GetComponent<DeathHandler>().Death();
        }
    }
}
