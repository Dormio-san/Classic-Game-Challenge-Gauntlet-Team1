using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    // Add variables to reference the player so the camera can follow them.
    private GameObject player;
    private Transform playerTransform;

    public float cameraSpeed = 0.125f; // Adjusts how smoothly the camera moves.
    
    // Bools used to determine when things occur below.
    public bool playerSpawning = false;
    public bool playerMoving = false;
    
    void LateUpdate()
    {
        // If the player is spawned, find them and assign the variables that reference them.
        if (playerSpawning)
        {
            player = GameObject.FindWithTag("Player");
            playerTransform = player.GetComponent<Transform>();
            playerSpawning = false; // Make this false so this if does not run again unless required.
            playerMoving = true; // Set this to true so the next if can be constantly updating while the player is in the level.
        }    
        
        // If the player is moving or is in the level, perform operations to make the camera follow them.
        if (playerMoving)
        {
           // Take the player's x and y position and make that the new position of the camera. Maintain the cameras Z since this is 2D.
            Vector3 desiredPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
            
            
            // Take the desired position from above and make the transition to it smoother by using lerp and camera speed variable to make the movement from the camera's old position to its new position smooth.
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, cameraSpeed);
            
            // Finally, set the camera's position to the newly created smoothedPosition.
            transform.position = smoothedPosition;
        }
    }
}