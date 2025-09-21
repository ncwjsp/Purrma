using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject[] yarnPrefabs; // drag red/green/blue here
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.2f;
    [SerializeField] private float bulletSpeed = 10f; // Speed of the bullet
    [SerializeField] private AudioSource shootSound; // 🔊 assign in Inspector
    
    [Header("Next Yarn Display")]
    [SerializeField] private Transform nextYarnHolder; // Empty GameObject where the cat holds the next yarn
    [SerializeField] private float holderScale = 0.7f; // Scale of the held yarn (smaller than shot yarn)

    private float lastShotTime = Mathf.NegativeInfinity;
    private GameObject nextYarnPrefab;
    private GameObject heldYarnInstance;

    private void Start()
    {
        // Generate the first next yarn
        GenerateNextYarn();
    }

    public void OnFire(InputValue value)
    {
        // Don't allow shooting if game is over or won
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver() || GameManager.Instance.IsWin()))
            return;
            
        // Don't allow shooting if mouse is over UI elements (like pause button)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
            
        if (value.isPressed && Time.time >= lastShotTime + fireCooldown)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    private void Shoot()
    {
        if (nextYarnPrefab == null)
        {
            Debug.LogWarning("No next yarn prefab available!");
            return;
        }

        // Create the bullet from the held yarn
        GameObject bullet = Instantiate(nextYarnPrefab, firePoint.position, firePoint.rotation);
        
        // Set up the bullet properly
        YarnBullet bulletScript = bullet.GetComponent<YarnBullet>();
        Yarn yarnScript = bullet.GetComponent<Yarn>();
        
        // Set the bullet speed before firing
        bulletScript.SetSpeed(bulletSpeed);
        
        // Set the color based on the prefab name
        if (yarnScript != null)
        {
            string prefabName = nextYarnPrefab.name;
            if (prefabName.Contains("Red"))
                yarnScript.SetColor("Red");
            else if (prefabName.Contains("Green"))
                yarnScript.SetColor("Green");
            else if (prefabName.Contains("Blue"))
                yarnScript.SetColor("Blue");
            else if (prefabName.Contains("Pink"))
                yarnScript.SetColor("Pink");
            else if (prefabName.Contains("Purple"))
                yarnScript.SetColor("Purple");
            else if (prefabName.Contains("Yellow"))
                yarnScript.SetColor("Yellow");
            else
                yarnScript.SetColor("Unknown");
        }
        
        bulletScript.Fire(firePoint.up);

        // Generate the next yarn for the cat to hold
        GenerateNextYarn();

        // 🔊 Play sound
        if (shootSound != null)
            shootSound.Play();
    }

    private void GenerateNextYarn()
    {
        // Choose next yarn randomly
        nextYarnPrefab = yarnPrefabs[Random.Range(0, yarnPrefabs.Length)];
        
        // Update the held yarn display
        UpdateHeldYarn();
    }

    private void UpdateHeldYarn()
    {
        if (nextYarnPrefab == null || nextYarnHolder == null) return;

        // Destroy previous held yarn
        if (heldYarnInstance != null)
        {
            Destroy(heldYarnInstance);
        }

        // Create new held yarn
        heldYarnInstance = Instantiate(nextYarnPrefab, nextYarnHolder.position, nextYarnHolder.rotation);
        heldYarnInstance.transform.SetParent(nextYarnHolder);
        heldYarnInstance.transform.localScale = Vector3.one * holderScale;
        
        // Disable physics and colliders for the held yarn
        var rb = heldYarnInstance.GetComponent<Rigidbody2D>();
        if (rb != null) 
        {
            rb.simulated = false;
            rb.isKinematic = true;
        }
        
        var collider = heldYarnInstance.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        // Disable the YarnBullet script so it doesn't try to move
        var bulletScript = heldYarnInstance.GetComponent<YarnBullet>();
        if (bulletScript != null) bulletScript.enabled = false;
        
        // Make sure it doesn't get destroyed automatically
        var destroyScript = heldYarnInstance.GetComponent<MonoBehaviour>();
        if (destroyScript != null && destroyScript.GetType().Name == "DestroySelf")
        {
            destroyScript.enabled = false;
        }
    }

    // Public method to get the next yarn color (useful for other scripts)
    public string GetNextYarnColor()
    {
        if (nextYarnPrefab == null) return "Unknown";
        
        string prefabName = nextYarnPrefab.name;
        if (prefabName.Contains("Red"))
            return "Red";
        else if (prefabName.Contains("Green"))
            return "Green";
        else if (prefabName.Contains("Blue"))
            return "Blue";
        else if (prefabName.Contains("Pink"))
            return "Pink";
        else if (prefabName.Contains("Purple"))
            return "Purple";
        else if (prefabName.Contains("Yellow"))
            return "Yellow";
        else
            return "Unknown";
    }

    // Public methods to control bullet speed
    public void SetBulletSpeed(float newSpeed)
    {
        bulletSpeed = newSpeed;
        Debug.Log($"Bullet speed set to: {bulletSpeed}");
    }

    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }
}
