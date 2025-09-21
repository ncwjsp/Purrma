using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverTitle;
    [SerializeField] private TextMeshProUGUI remainingYarnsText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float buttonDelay = 0.3f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource buttonClickSound;
    
    private CanvasGroup panelCanvasGroup;
    private GameManager gameManager;
    private YarnChainManager chainManager;
    
    private void Start()
    {
        // Get components
        gameManager = FindObjectOfType<GameManager>();
        chainManager = FindObjectOfType<YarnChainManager>();
        panelCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
        
        // Initially hide the panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Setup button listeners
        SetupButtons();
        
        // Subscribe to game over event
        YarnChainManager.OnGameOver += ShowGameOverScreen;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        YarnChainManager.OnGameOver -= ShowGameOverScreen;
    }
    
    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => {
                PlayButtonSound();
                RestartGame();
            });
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(() => {
                PlayButtonSound();
                GoToMainMenu();
            });
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => {
                PlayButtonSound();
                QuitGame();
            });
        }
    }
    
    private void ShowGameOverScreen()
    {
        if (gameOverPanel == null) return;
        
        // Show the panel
        gameOverPanel.SetActive(true);
        
        // Update UI text
        UpdateGameOverText();
        
        // Start fade in animation
        StartCoroutine(FadeInPanel());
        
        // Show buttons with delay
        StartCoroutine(ShowButtonsWithDelay());
    }
    
    private void UpdateGameOverText()
    {
        // Update title
        if (gameOverTitle != null)
        {
            gameOverTitle.text = "Game Over!";
        }
        
        
        // Update remaining yarns
        if (remainingYarnsText != null && chainManager != null)
        {
            remainingYarnsText.text = $"Yarns Remaining: {chainManager.GetRemainingYarns()}";
        }
    }
    
    private System.Collections.IEnumerator FadeInPanel()
    {
        if (panelCanvasGroup == null) yield break;
        
        panelCanvasGroup.alpha = 0f;
        
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        
        panelCanvasGroup.alpha = 1f;
    }
    
    private System.Collections.IEnumerator ShowButtonsWithDelay()
    {
        // Initially hide all buttons
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(false);
        if (quitButton != null) quitButton.gameObject.SetActive(false);
        
        // Show buttons one by one with delay
        yield return new WaitForSeconds(buttonDelay);
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(buttonDelay);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(buttonDelay);
        }
        
        if (quitButton != null)
        {
            quitButton.gameObject.SetActive(true);
        }
    }
    
    private void RestartGame()
    {
        Debug.Log("Restarting game from GameOverUI...");
        
        // Hide the game over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // Call game manager restart
        if (gameManager != null)
            gameManager.RestartGame();
    }
    
    private void GoToMainMenu()
    {
        Debug.Log("Going to main menu from GameOverUI...");
        SceneManager.LoadScene("Main Menu");
    }
    
    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        // For editor testing
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    private void PlayButtonSound()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
    }
    
    // Public method to hide the game over screen (useful for restart)
    public void HideGameOverScreen()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
