using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{

    public PlayerController Player;
    public Vector3 RespawnPosition;
    public float LingerOnDeath;
    public float CameraDelay;
    public float RespawnTimer;
    public CameraBehaviour Camera;
    public float DeathZoom;
    
    private float nextRespawn;
    private bool dead;
    private float counter;
    private float lingerTimer;
    private float cameraTimer;
    private bool staticCamera = false;


    // Use this for initialization
    void Start()
    {
        dead = false;
        staticCamera = false;
    }

    private void Update()
    {
        if (dead) {
            lingerTimer += Time.deltaTime;
            if (lingerTimer < LingerOnDeath)
                return;

            cameraTimer += Time.deltaTime;
            if (cameraTimer < CameraDelay)
                return;

            if (!staticCamera) {
                Debug.Log("Camera should move!");
                Camera.ZoomControl(DeathZoom * -1);
                Camera.StaticCameraControl(RespawnPosition + new Vector3(0, 2, -10));
                staticCamera = true;
            }

            counter += Time.deltaTime;
            if (counter < nextRespawn)
                return;

            Debug.Log("Respawned");
            dead = false;
            staticCamera = false;
            Player.gameObject.GetComponent<MeshRenderer>().enabled = true;
            Player.gameObject.GetComponent<PlayerController>().enabled = true;
            transform.position = RespawnPosition;
            Camera.StaticCameraControl(Vector3.zero);
            counter = 0;
            lingerTimer = 0;
            cameraTimer = 0;

        }

    }

    public void Death()
    {
        Debug.Log("Player is Dead");
        Player.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Player.gameObject.GetComponent<PlayerController>().enabled = false;
        dead = true;
        nextRespawn = counter + RespawnTimer;
        Camera.ZoomControl(DeathZoom);
    }

    public void SetRespawnPosition(Vector3 newPos)
    {
        RespawnPosition = newPos;
        Debug.Log("Checkpoint, RespawnPosition set to: " + RespawnPosition);
    }


}
