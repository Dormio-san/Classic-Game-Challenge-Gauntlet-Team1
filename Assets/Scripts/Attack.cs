using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject explosionAnimation;
    private PlayerBehavior pB;
    private int enemyHitScore = 10;
    private int damageDealt = 1;

    // Start is called before the first frame update
    void Start()
    {
        pB = GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag("Enemy"))
        {
            Instantiate(explosionAnimation, transform.position, Quaternion.identity);
            hit.GetComponent<EnemyBehavior>().EnemyTakeDamage(damageDealt);
            pB.PlayerScoreChange(enemyHitScore);          
            Destroy(this.gameObject);
        }
        else if (hit.CompareTag("Wall") || hit.CompareTag("Door"))
        {
            Destroy(this.gameObject);
        }
        else if (hit.CompareTag("Spawner"))
        {
            pB.PlayerScoreChange(enemyHitScore);
        }
    }
}