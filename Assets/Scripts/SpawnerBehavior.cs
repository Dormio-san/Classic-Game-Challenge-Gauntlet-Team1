using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    private int spawnerType; // Int that classifies different spawners as different types so that different enemies spawn.
    private GameManager gM; // Variable that references the GameManager script (assigned below).
    public GameObject enemyGhost; // The enemy ghost object that spawns.
    public int spawnerHealth = 3; // Int that gives the spawner health.
    private SpriteRenderer spriteRenderer; // Used to get the sprite renderer component of the spawner.

    // The different sprites for the spawner that are used when it takes damage.
    private static Sprite levelTwoSprite;
    private static Sprite levelOneSprite;

    // Sprites for ghost spawner.
    public Sprite ghostSpawnerLevelTwo;
    public Sprite ghostSpawnerLevelOne;
   

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Spawner health: " + spawnerHealth); // Used for testing, but will be commented out at end.

        // Assign game manager script reference and sprite renderer reference.
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If the gameObject this script is attached to has the name "GhostSpawner" give it spawnerType 1 and Begin Spawning.
        if (this.gameObject.name == "GhostSpawner")
        {
            spawnerType = 1;
        }
        
        BeginSpawning();
    }

    void Update()
    {
        // When the game is over, cancel the invoke, or spawning of enemies.
        if (gM.isGameOver)
        {
            CancelInvoke();
        }
    }

    // Begin the process of spawning enemies by running the correct invoke function.
    void BeginSpawning()
    {
        // If spawnerType 1 (GhostSpawner) run the SpawnGhosts function 5 seconds after the game starts and every 3.5 seconds after.
        if (spawnerType == 1)
        {
            levelTwoSprite = ghostSpawnerLevelTwo;
            levelOneSprite = ghostSpawnerLevelOne;
            InvokeRepeating("SpawnGhosts", 5.0f, 3.5f);            
        }
    }

    // Spawn the ghosts for the ghost spawner.
    void SpawnGhosts()
    {
        Instantiate(enemyGhost, transform.position, Quaternion.identity);
        Instantiate(enemyGhost, transform.position, Quaternion.identity);
    }

    // Function that will run when the spawner takes damage to check what will happen based on its health.
    // In each if statement, the spriteRenderer changes the sprite (what the spawner looks like) depending on how much health is left.
    public void TookDamage(int damageTaken)
    {
        spawnerHealth -= damageTaken;

        if (spawnerHealth == 2)
        {
            spriteRenderer.sprite = levelTwoSprite;
            Debug.Log("Spawner health: " + spawnerHealth);
        }
        else if (spawnerHealth == 1)
        {
            spriteRenderer.sprite = levelOneSprite;
            Debug.Log("Spawner health: " + spawnerHealth);
        }
        else if (spawnerHealth == 0)
        {
            Debug.Log("Spawner health: " + spawnerHealth + " object destroyed.");
            Destroy(this.gameObject);
        }
    }
}