using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    
    [Header("Pause Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private bool canPause = true;
    
    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    
    [Header("Audio")]
    [SerializeField] private AudioSource pauseSound;
    [SerializeField] private AudioSource resumeSound;
    
    private bool isPaused = false;
    private float previousTimeScale = 1f;
    
    // Player input components
    private PlayerShooting playerShooting;
    private PlayerAiming playerAiming;
    private GameObject playerObject;
    
    // Events
    public static System.Action OnGamePaused;
    public static System.Action OnGameResumed;
    
    private void Awake()
    {
        // Set singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Find player components
        playerShooting = FindObjectOfType<PlayerShooting>();
        playerAiming = FindObjectOfType<PlayerAiming>();
        
        // Find player object
        if (playerShooting != null)
            playerObject = playerShooting.gameObject;
        
        // Initialize UI
        if (pausePanel != null)
            pausePanel.SetActive(false);
            
        if (pauseButton != null)
            pauseButton.SetActive(true);
    }
    
    private void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(pauseKey) && canPause)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        if (isPaused || !canPause) return;
        
        // Don't pause if game is over or won
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver() || GameManager.Instance.IsWin())
                return;
        }
        
        isPaused = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        
        // Disable player input
        DisablePlayerInput();
        
        // Show pause UI
        if (pausePanel != null)
            pausePanel.SetActive(true);
            
        if (pauseButton != null)
            pauseButton.SetActive(false);
        
        // Play pause sound
        if (pauseSound != null)
            pauseSound.Play();
        
        // Invoke event
        OnGamePaused?.Invoke();
        
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        isPaused = false;
        Time.timeScale = previousTimeScale;
        
        // Re-enable player input
        EnablePlayerInput();
        
        // Hide pause UI
        if (pausePanel != null)
            pausePanel.SetActive(false);
            
        if (pauseButton != null)
            pauseButton.SetActive(true);
        
        // Play resume sound
        if (resumeSound != null)
            resumeSound.Play();
        
        // Invoke event
        OnGameResumed?.Invoke();
        
        Debug.Log("Game Resumed");
    }
    
    public void RestartGame()
    {
        if (!isPaused) return;
        
        // Resume game first
        ResumeGame();
        
        // Restart the game through GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        
        Debug.Log("Game Restarted from Pause");
    }
    
    public void GoToMainMenu()
    {
        if (!isPaused) return;
        
        // Resume game first to restore time scale
        ResumeGame();
        
        // Go to main menu through GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        
        Debug.Log("Going to Main Menu from Pause");
    }
    
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
    
    public void SetCanPause(bool canPause)
    {
        this.canPause = canPause;
    }
    
    private void DisablePlayerInput()
    {
        // Disable player input completely
        if (playerObject != null)
        {
            // Disable the entire player GameObject
            playerObject.SetActive(false);
        }
        else
        {
            // Fallback: disable individual components
            if (playerShooting != null)
                playerShooting.enabled = false;
                
            if (playerAiming != null)
                playerAiming.enabled = false;
        }
    }
    
    private void EnablePlayerInput()
    {
        // Re-enable player input
        if (playerObject != null)
        {
            // Re-enable the entire player GameObject
            playerObject.SetActive(true);
        }
        else
        {
            // Fallback: re-enable individual components
            if (playerShooting != null)
                playerShooting.enabled = true;
                
            if (playerAiming != null)
                playerAiming.enabled = true;
        }
    }
    
    private void OnDestroy()
    {
        // Ensure time scale is restored when object is destroyed
        if (isPaused)
        {
            Time.timeScale = previousTimeScale;
        }
    }
}
