using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraZone : MonoBehaviour {

    private Vector3 CameraPosition;
    public CameraBehaviour Camera;

    private void Awake()
    {
        CameraPosition = (Vector3)this.gameObject.transform.GetChild(0).position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {    
        if (other.gameObject.CompareTag("Player")){
            Debug.Log(this.gameObject.name + ": " + CameraPosition);
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
