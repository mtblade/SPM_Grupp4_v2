using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour {

    public Vector3 CameraPosition;
    public CameraBehaviour Camera;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision");
        if (other.gameObject.CompareTag("Player")){
            Camera.StaticCameraControl(CameraPosition);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Camera.StaticCameraControl(CameraPosition);
        }
    }
}
