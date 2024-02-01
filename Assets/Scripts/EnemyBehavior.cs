using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private int enemyType;

    // Variables based on certain enemy type.    
    private int enemyHealth;
    private float enemyMoveSpeed;
    private int damageToPlayer; // The value of this variable is handled in player behavior because player class determines damage taken.

    // Ghost enemy variables.
    private int ghostHealth = 1;
    private float ghostMoveSpeed = 3.5f;  
    
    
    private GameObject player;
    private Transform playerTransform;
    private PlayerBehavior pB;

    private GameManager gM;

    void Start()
    {
        //enemyRenderer = GetComponent<Renderer>();

        player = GameObject.FindWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
        pB = player.GetComponent<PlayerBehavior>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Check the name of the object. If it is the ghost, give it enemy type 1 and then set the variables for type 1.
        if (this.name == ("Ghost(Clone)"))
        {
            enemyType = 1;
        }
        SetEnemySpecificVariables();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();        
    }

    void SetEnemySpecificVariables()
    {
        if (enemyType == 1)
        {
            // Ghost is enemy type 1, so set the variables to ghost values.
            enemyHealth = ghostHealth;
            enemyMoveSpeed = ghostMoveSpeed;
            damageToPlayer = pB.ghostDamagePlayerTakes;
        }
    }

    void EnemyMovement()
    {
        if (playerTransform != null)
        {
            // Calculate the direction from the enemy to the player
            Vector3 direction = playerTransform.position - transform.position;

            // Normalize the direction to get a unit vector
            Vector3 normalizedDirection = direction.normalized;

            // Move the enemy towards the player
            transform.position += normalizedDirection * enemyMoveSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D smacked)
    {
        if (smacked.CompareTag("Player"))
        {
            pB.PlayerHealthChange(damageToPlayer);
            Destroy(this.gameObject);
        }
    }

    public void EnemyTakeDamage(int damageTaken)
    {
        enemyHealth -= damageTaken;

        if (enemyHealth == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
