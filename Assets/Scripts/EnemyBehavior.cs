using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    int enemyHealth;
    float enemyMoveSpeed;
    int enemyType;
    int ghostDamage = -7;
    GameObject player;
    Transform playerTransform;
    PlayerBehavior pB;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player(Clone)");
        playerTransform = player.GetComponent<Transform>();
        pB = player.GetComponent<PlayerBehavior>();
        SetEnemySpecificVariables(); 
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    void SetEnemySpecificVariables()
    {
        // Ghost
        //enemyHealth = 1;
        enemyMoveSpeed = 3.5f;
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
            pB.PlayerHealthChange(ghostDamage);
        }
    }
}
