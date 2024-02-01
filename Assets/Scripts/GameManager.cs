using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//Debugs used in script are for testing unless otherwise specified.
public class GameManager : MonoBehaviour
{
    public GameObject playerSpawnAnimation;
    private float animationWaitTime = 1.5f;
    public GameObject playerAvatar;
    // Not sure if spawning will occur in this script or will be preset through Unity.
    // public GameObject enemyGhostSpawner;
    public GameObject enemyGhost;
    // public GameObject chestItem;
    // public GameObject keyItem;
    // public GameObject potionItem;
    // public GameObject healingItem;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerScoreText;

    [HideInInspector] public bool isGameOver = false;
    private CameraBehavior cB;

    // Temporary way of handling key image update on UI.
    public GameObject[] keyImages;
    public GameObject[] potionImages;


    void Start()
    {
        cB = GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>();
        StartCoroutine("SpawnLevelOne");
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
        Instantiate(playerSpawnAnimation, new Vector3(0, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(animationWaitTime);
        Instantiate(playerAvatar, new Vector3(0, 0, 0), Quaternion.identity);
        
        cB.playerSpawning = true;
        
        Instantiate(enemyGhost, new Vector3(8, -3, 0), Quaternion.identity);
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game is over!");
        ChangeScene("Engineer_Testing");
    }

    public void ChangeHealthText(int currentHealth)
    {
        playerHealthText.text = "Health: " + currentHealth;
    }

    public void ChangeScoreText(int currentScore)
    {
        playerScoreText.text = "Score: " + currentScore;
    }

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

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}