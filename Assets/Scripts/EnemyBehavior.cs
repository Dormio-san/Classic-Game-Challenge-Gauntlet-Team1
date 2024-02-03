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
    private int damageToEnemy; // This is used when the enemy hits the player.

    // Ghost enemy variables.
    private int ghostHealth = 1;
    private float ghostMoveSpeed = 2.5f;
    private int damageToGhost = 1;
    
    
    // References to the player
    private GameObject player;
    private Transform playerTransform;
    private PlayerBehavior pB;

    // Reference to game manager.
    private GameManager gM;

    void Start()
    {
        // Assign the variables above their references.
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

    // Run the enemy's movement function.
    void Update()
    {
        EnemyMovement();        
    }

    // Do as name suggests based on conditions with the function.
    void SetEnemySpecificVariables()
    {
        if (enemyType == 1)
        {
            // Ghost is enemy type 1, so set the variables to ghost values.
            enemyHealth = ghostHealth;
            enemyMoveSpeed = ghostMoveSpeed;
            damageToPlayer = pB.ghostDamagePlayerTakes; // Need reference to player script here because the damage the player takes varies based on their class, which is set in the player script.
            damageToEnemy = damageToGhost;
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

    // On collision, check the collided with objects tag.
    void OnTriggerEnter2D(Collider2D smacked)
    {
        // If enemy hits player, deal damage to the player and deal damage to the enemy.
        if (smacked.tag == "Player")
        {
            pB.PlayHitEnemy();
            pB.PlayerHealthChange(damageToPlayer);
            EnemyTakeDamage(damageToEnemy);
        }
        else if (smacked.tag == "Wall")
        {
            transform.position = new Vector2 (transform.position.x, transform.position.y);
        }
        else if (smacked.tag == "Door")
        {
            transform.position = new Vector2(transform.position.x, transform.position.y);
        }
    }

    // Function that makes the enemy take damage and checks to see when the enemy should be destroyed.
    public void EnemyTakeDamage(int damageTaken)
    {
        enemyHealth -= damageTaken;

        if (enemyHealth == 0)
        {
            Destroy(this.gameObject);
        }
    }
}