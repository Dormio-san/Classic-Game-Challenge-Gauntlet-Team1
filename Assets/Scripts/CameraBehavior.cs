using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private GameObject player;
    Transform playerTransform;
    public float cameraSpeed = 0.125f; // Adjusts how smoothly the camera moves.
    public bool playerSpawned = false;
    public bool playerMoving = false;

    void Start()
    {
        
    }
    void LateUpdate()
    {
        if (playerSpawned)
        {
            player = GameObject.FindWithTag("Player");
            playerTransform = player.GetComponent<Transform>();
            playerSpawned = false;
            playerMoving = true;            
        }    
        if (playerMoving)
        {
            Vector3 desiredPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, cameraSpeed);
            transform.position = smoothedPosition;
        }
    }
}