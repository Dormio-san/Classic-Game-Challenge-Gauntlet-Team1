using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//Debugs used in script are for testing unless otherwise specified.
public class GameManager : MonoBehaviour
{
    // Player related variables.
    public GameObject playerAvatar;
    public GameObject playerSpawnAnimation;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerScoreText;

    private float animationWaitTime = 1.5f; // Used for wait time from start of animation to spawning of player.
    
    /* Not sure if spawning will occur in this script or will be preset through Unity.
    // public GameObject enemyGhostSpawner;
    // public GameObject enemyGhost;
    // public GameObject chestItem;
    // public GameObject keyItem;
    // public GameObject potionItem;
    // public GameObject healingItem;
     Will most likely delete the variables in this chunk.
    */

    [HideInInspector] public bool isGameOver = false; // Variables used to tell when the game is over and in turn run certain functions.

    private CameraBehavior cB; // Variable that is used to refer to the camera behavior script.

    // Way of handling image update on UI.
    public GameObject[] keyImages;
    public GameObject[] potionImages;


    void Start()
    {
        cB = GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>(); // Set reference to camera behavior script.

        StartCoroutine("SpawnLevelOne"); // Begin spawning for level one.
    }

    void Update()
    {
        // If escape key is pressed, exit the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // The Unity Editor line allows for testing in the non built version as it exits play mode in the editor.
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
            Debug.Log("Quitting game!");
        }        
    }

    IEnumerator SpawnLevelOne()
    {
        // Spawn various items for level one and set variable in camera behavior to true so it can find the player and follow them.
        Instantiate(playerSpawnAnimation, new Vector3(0, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(animationWaitTime);
        Instantiate(playerAvatar, new Vector3(0, 0, 0), Quaternion.identity);
        
        cB.playerSpawning = true;
        
        //Instantiate(enemyGhost, new Vector3(8, -3, 0), Quaternion.identity);
    }

    // GameOver function stops what is occurring and goes to the end game screen.
    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game is over!");
        ChangeScene("Engineer_Testing");
    }

    // Updates the UI for player health.
    public void ChangeHealthText(int currentHealth)
    {
        playerHealthText.text = "Health: " + currentHealth;
    }

    // Updates the UI for player score.
    public void ChangeScoreText(int currentScore)
    {
        playerScoreText.text = "Score: " + currentScore;
    }

    // Updates the keys UI based on the numberOfKeys the player has.
    public void ChangeKeysUI(int numberOfKeys)
    {
        // Whenever numberOfKeys decreases, turn off the existing images.
        foreach (GameObject aKeyImage in keyImages)
        {
            aKeyImage.SetActive(false);
        }

        // Then, update the images to only the number of keys that the player has. With all the images disabled, the for loop will run again and activate the current numberOfKeys.
        for (int i = 0; i < numberOfKeys && i < keyImages.Length; i++)
        {
            keyImages[i].SetActive(true);
        }
    }

    // Updates the potion UI based on the numberOfPotions the player has.
    public void ChangePotionUI(int numberOfPotions)
    {
        // Whenever numberOfPotions decreases, turn off the existing images.
        foreach (GameObject aPotionImage in potionImages)
        {
            aPotionImage.SetActive(false);
        }

        // Then, update the images to only the number of potions that the player has. With all the images disabled, the for loop will run again and activate the current numberOfPotions.
        for (int i = 0; i < numberOfPotions && i < potionImages.Length; i++)
        {
            potionImages[i].SetActive(true);
        }
    }

    // Changes the scene to the sceneName provided when the function is called.
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}