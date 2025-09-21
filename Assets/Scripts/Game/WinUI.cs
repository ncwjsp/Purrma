using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WinUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winTitle;
    [SerializeField] private TextMeshProUGUI winMessage;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float buttonDelay = 0.3f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource buttonClickSound;
    [SerializeField] private AudioSource winSound;
    
    private CanvasGroup panelCanvasGroup;
    private GameManager gameManager;
    private YarnChainManager chainManager;
    private bool hasPlayedWinSound = false;
    
    private void Start()
    {
        // Get components
        gameManager = FindObjectOfType<GameManager>();
        chainManager = FindObjectOfType<YarnChainManager>();
        panelCanvasGroup = winPanel.GetComponent<CanvasGroup>();
        
        // Initially hide the panel
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        
        // Setup button listeners
        SetupButtons();
        
        // Subscribe to win event
        YarnChainManager.OnWin += ShowWinScreen;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        YarnChainManager.OnWin -= ShowWinScreen;
    }
    
    private void SetupButtons()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(() => {
                PlayButtonSound();
                NextLevel();
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
    
    private void ShowWinScreen()
    {
        if (winPanel == null) return;
        
        // Play win sound only once
        if (winSound != null && !hasPlayedWinSound)
        {
            // Ensure the sound doesn't loop
            winSound.loop = false;
            winSound.Play();
            hasPlayedWinSound = true;
        }
        
        // Show the panel
        winPanel.SetActive(true);
        
        // Update UI text
        UpdateWinText();
        
        // Start fade in animation
        StartCoroutine(FadeInPanel());
        
        // Show buttons with delay
        StartCoroutine(ShowButtonsWithDelay());
    }
    
    private void UpdateWinText()
    {
        // Update title
        if (winTitle != null)
        {
            winTitle.text = "You Win!";
        }
        
        // Update win message
        if (winMessage != null)
        {
            winMessage.text = "Congratulations! You cleared all the yarns!";
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
        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(false);
        if (quitButton != null) quitButton.gameObject.SetActive(false);
        
        // Show buttons one by one with delay
        yield return new WaitForSeconds(buttonDelay);
        
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);
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
    
    private void NextLevel()
    {
        Debug.Log("Going to next level...");
        
        // Hide the win panel
        if (winPanel != null)
            winPanel.SetActive(false);
            
        // Call game manager to go to next level
        if (gameManager != null)
            gameManager.NextLevel();
    }
    
    private void GoToMainMenu()
    {
        Debug.Log("Going to main menu from WinUI...");
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
    
    // Public method to hide the win screen (useful for next level)
    public void HideWinScreen()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }
    
    // Reset win sound flag for new game
    public void ResetWinSound()
    {
        hasPlayedWinSound = false;
    }
}
