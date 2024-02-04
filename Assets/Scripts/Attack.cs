using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure this script is assigned to each of the player weapons (the arrow weapon used for each class).
public class Attack : MonoBehaviour
{
    //public GameObject explosionAnimation; // This needs to be assigned in the inspector of the player weapon. (Not doing).
    private PlayerBehavior pB; // This is a reference to the player behavior script which is used in different parts of the script.
    private int enemyHitScore = 10; // This is the amount of score the player is given when they hit an enemy.
    private int damageDealt = 1; // This is the amount of damage the player's weapon deals to enemies.

    void Start()
    {
        // Assign the reference to the player script.
        pB = GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>();
    }

    // When the weapon collides with something, check the tag to see what it is. 
    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag("Enemy"))
        {
            // If the arrow hits an enemy, play the explosion animation (not doing), deal damage to the enemy, change the player's score, and destroy the arrow.
            pB.PlayAttackHit();
            //Instantiate(explosionAnimation, transform.position, Quaternion.identity);
            hit.GetComponent<EnemyBehavior>().EnemyTakeDamage(damageDealt);
            pB.PlayerScoreChange(enemyHitScore);          
            Destroy(this.gameObject);
        }
        else if (hit.CompareTag("Wall") || hit.CompareTag("Door"))
        {
            // If the arrow hits a wall or door, destroy the arrow when it collides.
            Destroy(this.gameObject);
        }
        else if (hit.CompareTag("GhostSpawner"))
        {
            // If the arrow hits the spawner, give the player score, run the TookDamage function in the spawner script, and destroy the arrow.
            pB.PlayAttackHit();
            hit.GetComponent<SpawnerBehavior>().TookDamage(damageDealt);
            pB.PlayerScoreChange(enemyHitScore);
            Destroy(this.gameObject);
        }
    }
}