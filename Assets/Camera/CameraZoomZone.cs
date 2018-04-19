using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomZone : MonoBehaviour {

    public float ZoomDistance;
    public CameraBehaviour Camera;
    private bool zooming = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Entered ZoomZone!");
            Camera.ZoomControl(ZoomDistance);
            zooming = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !zooming) {
            Debug.Log("Entered ZoomZone!");
            Camera.ZoomControl(ZoomDistance);
            zooming = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Leaving ZoomZone!");
            Camera.ZoomControl(ZoomDistance * -1);
            zooming = false;
        }
    }
}
