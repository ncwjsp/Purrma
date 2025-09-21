using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI remainingYarnsText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Win UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button winMainMenuButton;
    
    [Header("Game Settings")]
    [SerializeField] private float gameOverDelay = 1f; // Delay before showing game over screen
    
    [Header("Audio")]
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private AudioSource backgroundMusic;
    
    [Header("Pause System")]
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private PauseUI pauseUI;
    
    private YarnChainManager chainManager;
    private PlayerShooting playerShooting;
    private PlayerAiming playerAiming;
    private GameObject playerObject;
    private WinUI winUI;
    private bool isGameOver = false;
    private bool isWin = false;
    
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
        // Find components
        chainManager = FindObjectOfType<YarnChainManager>();
        playerShooting = FindObjectOfType<PlayerShooting>();
        playerAiming = FindObjectOfType<PlayerAiming>();
        winUI = FindObjectOfType<WinUI>();
        pauseManager = FindObjectOfType<PauseManager>();
        pauseUI = FindObjectOfType<PauseUI>();
        
        // Find player object
        if (playerShooting != null)
            playerObject = playerShooting.gameObject;
        
        // Subscribe to events
        YarnChainManager.OnGameOver += HandleGameOver;
        YarnChainManager.OnWin += HandleWin;
        
        // Initialize UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        if (winPanel != null)
            winPanel.SetActive(false);
            
        // Start background music
        if (backgroundMusic != null)
            backgroundMusic.Play();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        YarnChainManager.OnGameOver -= HandleGameOver;
        YarnChainManager.OnWin -= HandleWin;
    }
    
    private void Update()
    {
        // Update UI during gameplay (only if not paused)
        if (!isGameOver && !isWin && !IsGamePaused())
        {
            UpdateGameUI();
        }
    }
    
    private void UpdateGameUI()
    {
        if (remainingYarnsText != null && chainManager != null)
        {
            remainingYarnsText.text = $"Yarns Remaining: {chainManager.GetRemainingYarns()}";
        }
    }
    
    private void HandleGameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("GameManager: Game Over received!");
        
        // Stop any existing coroutines to prevent multiple game over screens
        StopAllCoroutines();
        
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
        
        // Hide pause button when game is over
        if (pauseUI != null)
        {
            pauseUI.ShowPauseButton(false);
        }
            
        // Stop background music
        if (backgroundMusic != null)
            backgroundMusic.Stop();
            
        // Play game over sound
        if (gameOverSound != null)
            gameOverSound.Play();
            
        // Show game over screen after delay
        StartCoroutine(ShowGameOverScreen());
    }
    
    private void HandleWin()
    {
        if (isWin || isGameOver) return; // Prevent multiple triggers
        
        isWin = true;
        Debug.Log("GameManager: Win received!");
        
        // Disable player input completely
        if (playerObject != null)
        {
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
        
        // Hide pause button when game is won
        if (pauseUI != null)
        {
            pauseUI.ShowPauseButton(false);
        }
        
        // Stop background music
        if (backgroundMusic != null)
            backgroundMusic.Stop();
            
        // Show win screen
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            
            // Update win text
            if (winText != null)
            {
                winText.text = "You Win!";
            }
        }
        
        // Setup win button listeners
        SetupWinButtons();
    }
    
    
    private IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(gameOverDelay);
        
        // Double-check that we're still in game over state
        if (!isGameOver) yield break;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Update game over text
            if (gameOverText != null)
            {
                gameOverText.text = "Game Over!";
            }
            
            
            // Update remaining yarns
            if (remainingYarnsText != null && chainManager != null)
            {
                remainingYarnsText.text = $"Yarns Remaining: {chainManager.GetRemainingYarns()}";
            }
        }
        
        // Setup button listeners
        SetupGameOverButtons();
    }
    
    private void SetupWinButtons()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(NextLevel);
        }
        
        if (winMainMenuButton != null)
        {
            winMainMenuButton.onClick.RemoveAllListeners();
            winMainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }
    
    private void SetupGameOverButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        
        // Reset game state
        isGameOver = false;
        isWin = false;
        
        // Reset win sound
        if (winUI != null)
            winUI.ResetWinSound();
        
        // Reset chain manager
        if (chainManager != null)
        {
            chainManager.ResetGame();
        }
        
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
        
        // Show pause button when restarting
        if (pauseUI != null)
        {
            pauseUI.ShowPauseButton(true);
        }
            
        // Hide panels
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        if (winPanel != null)
            winPanel.SetActive(false);
            
        // Restart background music
        if (backgroundMusic != null)
            backgroundMusic.Play();
    }
    
    public void GoToMainMenu()
    {
        Debug.Log("Going to main menu...");
        SceneManager.LoadScene("Main Menu");
    }
    
    public void NextLevel()
    {
        Debug.Log("Going to next level...");
        
        // Get current scene name to determine next scene
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string nextSceneName = GetNextSceneName(currentSceneName);
        
        Debug.Log($"Current scene: {currentSceneName}, Next scene: {nextSceneName}");
        
        // Load the next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No next scene found! Going to main menu.");
            GoToMainMenu();
        }
    }
    
    private string GetNextSceneName(string currentScene)
    {
        // Remove .unity extension if present and convert to lowercase for comparison
        string sceneName = currentScene.ToLower().Replace(".unity", "");
        
        switch (sceneName)
        {
            case "easy":
                return "Medium";
            case "medium":
                return "Hard";
            case "hard":
                return "Main Menu"; // After completing all levels, go to main menu
            default:
                Debug.LogWarning($"Unknown scene: {currentScene}. Defaulting to Medium.");
                return "Medium";
        }
    }
    
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    public bool IsWin()
    {
        return isWin;
    }
    
    public bool IsGamePaused()
    {
        return pauseManager != null && pauseManager.IsPaused();
    }
    
    public void PauseGame()
    {
        if (pauseManager != null)
        {
            pauseManager.PauseGame();
        }
    }
    
    public void ResumeGame()
    {
        if (pauseManager != null)
        {
            pauseManager.ResumeGame();
        }
    }
}
