using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomZone : MonoBehaviour {

    public float ZoomDistance;
    public CameraBehaviour Camera;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log(this.gameObject.name + ": " + ZoomDistance);
            Camera.ZoomControl(ZoomDistance);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Camera.ZoomControl(ZoomDistance * -1);
        }
    }
}
