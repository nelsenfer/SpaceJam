using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public float knockbackForce = 15f;

    [Header("Bullet Type")]
    public bool isEnemyBullet = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
        else if (!isEnemyBullet && collision.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                enemyScript.DieAndKnockback(knockbackDir, knockbackForce);
            }
            Destroy(gameObject);
        }
        else if (isEnemyBullet && collision.CompareTag("Player"))
        {
            Debug.Log("PLAYER TERKENA TEMBAKAN! GAME OVER!");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}