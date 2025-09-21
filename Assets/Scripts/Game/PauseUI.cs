using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseUI : MonoBehaviour
{
    [Header("Pause UI Elements")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI pauseTitle;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Pause Button")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseButtonText;
    
    [Header("Audio")]
    [SerializeField] private AudioSource buttonClickSound;
    
    private void Start()
    {
        // Setup button listeners
        SetupButtons();
        
        // Initialize UI state
        if (pausePanel != null)
            pausePanel.SetActive(false);
            
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(true);
    }
    
    private void OnEnable()
    {
        // Subscribe to pause events
        PauseManager.OnGamePaused += OnGamePaused;
        PauseManager.OnGameResumed += OnGameResumed;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from pause events
        PauseManager.OnGamePaused -= OnGamePaused;
        PauseManager.OnGameResumed -= OnGameResumed;
    }
    
    private void SetupButtons()
    {
        // Setup pause button
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        
        // Setup resume button
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }
        
        // Setup restart button
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        
        // Setup main menu button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
    }
    
    private void OnPauseButtonClicked()
    {
        PlayButtonSound();
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.PauseGame();
        }
    }
    
    private void OnResumeButtonClicked()
    {
        PlayButtonSound();
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResumeGame();
        }
    }
    
    private void OnRestartButtonClicked()
    {
        PlayButtonSound();
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.RestartGame();
        }
    }
    
    private void OnMainMenuButtonClicked()
    {
        PlayButtonSound();
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.GoToMainMenu();
        }
    }
    
    private void OnGamePaused()
    {
        // Show pause panel
        if (pausePanel != null)
            pausePanel.SetActive(true);
            
        // Hide pause button
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false);
            
        // Update pause title
        if (pauseTitle != null)
            pauseTitle.text = "Game Paused";
    }
    
    private void OnGameResumed()
    {
        // Hide pause panel
        if (pausePanel != null)
            pausePanel.SetActive(false);
            
        // Show pause button
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(true);
    }
    
    private void PlayButtonSound()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
    }
    
    // Public methods for external access
    public void ShowPauseButton(bool show)
    {
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(show);
    }
    
    public void SetPauseButtonText(string text)
    {
        if (pauseButtonText != null)
            pauseButtonText.text = text;
    }
    
    public void SetPauseTitle(string title)
    {
        if (pauseTitle != null)
            pauseTitle.text = title;
    }
}
