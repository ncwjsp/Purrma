using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure we have a collider for solid collision detection
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = false; // Solid collision, not trigger
            collider.radius = 0.5f; // Make sure it's not too small
        }
        else
        {
            // Make sure existing collider is set as solid collision
            var existingCollider = GetComponent<Collider2D>();
            existingCollider.isTrigger = false; // Solid collision, not trigger
            if (existingCollider is CircleCollider2D circleCollider)
            {
                circleCollider.radius = 0.5f; // Ensure proper size
            }
        }
    }

    public void Fire(Vector2 direction)
    {
        rb.velocity = direction.normalized * speed;
        Debug.Log($"YarnBullet fired in direction: {direction.normalized} with speed: {speed}");
        Invoke(nameof(DestroySelf), lifeTime);
    }

    // Public method to set bullet speed from external scripts
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // Public method to get current bullet speed
    public float GetSpeed()
    {
        return speed;
    }

    private void Update()
    {
        // Backup manual collision detection for chain yarns
        CheckManualCollision();
    }

    private void CheckManualCollision()
    {
        // Find all yarns in the chain and check for manual collision
        YarnChainManager chainManager = FindObjectOfType<YarnChainManager>();
        if (chainManager == null) return;

        // Get all yarns from the chain manager
        var allYarns = chainManager.GetAllYarns();
        
        foreach (GameObject yarn in allYarns)
        {
            if (yarn == null) continue;
            
            // Check distance between bullet and yarn
            float distance = Vector2.Distance(transform.position, yarn.transform.position);
            float collisionRadius = 0.5f + 0.5f; // bullet radius + yarn radius
            
            if (distance < collisionRadius)
            {
                Debug.Log($"Manual collision detected with {yarn.name} at distance {distance}");
                
                // Trigger the same collision logic as OnCollisionEnter2D
                YarnChainNode hitNode = yarn.GetComponent<YarnChainNode>();
                if (hitNode != null && hitNode.chain != null)
                {
                    Debug.Log($"Manual hit yarn chain node at index {hitNode.index}");
                    
                    // Stop physics completely
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                    rb.simulated = false;

                    // Cancel destruction so it stays in the scene
                    CancelInvoke(nameof(DestroySelf));

                    // Disable the collider to prevent multiple hits
                    GetComponent<Collider2D>().enabled = false;

                    // Add to chain data structure BEFORE parenting
                    hitNode.chain.InsertYarnAt(hitNode, this.gameObject);
                    
                    // Disable this script to prevent further checks
                    enabled = false;
                    return;
                }
            }
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"YarnBullet hit: {collision.gameObject.name}");
        
        YarnChainNode hitNode = collision.gameObject.GetComponent<YarnChainNode>();
        if (hitNode != null && hitNode.chain != null)
        {
            Debug.Log($"Hit yarn chain node at index {hitNode.index}");
            
            // Stop physics completely
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.simulated = false;

            // Cancel destruction so it stays in the scene
            CancelInvoke(nameof(DestroySelf));

            // Disable the collider to prevent multiple hits
            GetComponent<Collider2D>().enabled = false;

            // Add to chain data structure BEFORE parenting
            hitNode.chain.InsertYarnAt(hitNode, this.gameObject);
        }
        else
        {
            Debug.Log($"Hit object {collision.gameObject.name} but no YarnChainNode found");
        }
    }
}
