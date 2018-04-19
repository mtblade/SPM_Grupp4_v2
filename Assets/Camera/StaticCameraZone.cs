using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraZone : MonoBehaviour {

    private Vector3 CameraPosition;
    public CameraBehaviour Camera;
    private bool _static = false;

    private void Awake()
    {
        CameraPosition = (Vector3)this.gameObject.transform.GetChild(0).position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {    
        if (other.gameObject.CompareTag("Player")){
            Debug.Log("Entered StaticCameraZone!");
            Camera.StaticCameraControl(CameraPosition);
            _static = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !_static) {
            Debug.Log("Entered StaticCameraZone!");
            Camera.StaticCameraControl(CameraPosition);
            _static = true;
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Left StaticCameraZone!");
            Camera.StaticCameraControl(CameraPosition);
            _static = false;
        }
    }
}
