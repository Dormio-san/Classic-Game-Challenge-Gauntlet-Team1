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

    // Bools related to the player's class that determine which avatar they use.
    private bool warriorClass;
    private bool valkyrieClass;
    private bool rangerClass;
    private bool wizardClass;

    // GameObjects for the different player classes.
    public GameObject warriorAvatar;
    public GameObject valkyrieAvatar;
    public GameObject rangerAvatar;
    public GameObject wizardAvatar;

    // May delete --> //private float animationWaitTime = 1.5f; // Used for wait time from start of animation to spawning of player.
    
    [HideInInspector] public bool isGameOver; // Variable used to tell when the game is over and in turn run certain functions.

    private CameraBehavior cB; // Variable that is used to refer to the camera behavior script.

    // Way of handling image update on UI.
    public GameObject[] keyImages;
    public GameObject[] potionImages;

    public static bool levelTwo; // Variable to change where player spawns based on the level they are in.

    void Start()
    {
        cB = GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>(); // Set reference to camera behavior script.

        SpawnPlayer(); // Spawn the player.

        // May delete this --> //StartCoroutine("SpawnPlayer"); // Begin spawning for level one.
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


    void SpawnPlayer()
    {
        // Set player's avatar since it applies to both if statements.
        SetPlayerAvatar();

        if (!levelTwo)
        {
            // Spawn player and then set the camera beahvior variable to true so that it can find the player and begin following them.            
            Instantiate(playerAvatar, new Vector2(24.23f, -15.29f), Quaternion.identity);
            CameraBehavior.playerSpawning = true;
        }
        else if (levelTwo)
        {
            // Spawn player and then set the camera beahvior variable to true so that it can find the player and begin following them.
            Instantiate(playerAvatar, new Vector2(27.50f, 12.05f), Quaternion.identity);
            CameraBehavior.playerSpawning = true;
        }
    }
    
    /*
      Commented out the Coroutine because we may not use it. If we have a spawn animation, it would be needed. Otherwise, it will be deleted.
    IEnumerator SpawnPlayer()
    {
        SetPlayerAvatar();
        // Spawn various items for level one and set variable in camera behavior to true so it can find the player and follow them.
        //Instantiate(playerSpawnAnimation, new Vector3(0, 0, 0), Quaternion.identity);
        //yield return new WaitForSeconds(animationWaitTime);
        Instantiate(playerAvatar, new Vector2(24.23f, -15.29f), Quaternion.identity);
        
        cB.playerSpawning = true;
        
        //Instantiate(enemyGhost, new Vector3(8, -3, 0), Quaternion.identity);
    }
    */

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

    // Sets the player class bools that are used to tell which class the player chose. This is called in the main menu script in order to set class specific values in here.
    public void SetPlayerClass(bool warriorChosen, bool valkyrieChosen, bool rangerChosen, bool wizardChosen)
    {
        warriorClass = warriorChosen;
        valkyrieClass = valkyrieChosen;
        rangerClass = rangerChosen;
        wizardClass = wizardChosen;
    }

    void SetPlayerAvatar()
    {
        SetPlayerClass(MainMenu.warrior, MainMenu.valkyrie, MainMenu.ranger, MainMenu.wizard);

        if (warriorClass)
        {
            playerAvatar = warriorAvatar;
        }
        else if (valkyrieClass)
        {
            playerAvatar = valkyrieAvatar;
        }
        else if (rangerClass)
        {
            playerAvatar = rangerAvatar;
        }
        else if (wizardClass)
        {
            playerAvatar = wizardAvatar;
        }
    }
}