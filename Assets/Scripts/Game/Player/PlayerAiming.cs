using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Needed for InputValue
using UnityEngine.EventSystems;

public class PlayerAiming : MonoBehaviour
{
    // [SerializeField] private float rotationSpeed = 720f;
    private Camera cam;
    private Vector2 lookPos;

    private void Awake()
    {
        cam = Camera.main;
    }

    // This is the ONLY signature that works with Send Messages for a Value action
    public void OnLook(InputValue value)
    {
        // Don't allow aiming if game is over or won
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver() || GameManager.Instance.IsWin()))
            return;
            
        // Don't allow aiming if mouse is over UI elements (like pause button)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
            
        lookPos = value.Get<Vector2>();
    }

    private void Update()
    {
        // Don't allow aiming if game is over or won
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver() || GameManager.Instance.IsWin()))
            return;
            
        // Don't allow aiming if mouse is over UI elements (like pause button)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
            
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(lookPos.x, lookPos.y, cam.nearClipPlane));
        Vector2 dir = mouseWorld - transform.position;
        dir.Normalize();

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

    }
}
