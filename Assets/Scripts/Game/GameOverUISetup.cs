using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to automatically set up the Game Over UI in the scene.
/// Attach this to any GameObject and run it in the editor to create the UI structure.
/// </summary>
public class GameOverUISetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool createUIAutomatically = false;
    
    [Header("References")]
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private GameObject gameOverPanel;
    
    private void Start()
    {
        if (createUIAutomatically)
        {
            SetupGameOverUI();
        }
    }
    
    [ContextMenu("Setup Game Over UI")]
    public void SetupGameOverUI()
    {
        // Create Canvas if it doesn't exist
        if (gameOverCanvas == null)
        {
            GameObject canvasGO = new GameObject("GameOverCanvas");
            gameOverCanvas = canvasGO.AddComponent<Canvas>();
            gameOverCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameOverCanvas.sortingOrder = 100; // High priority to appear on top
            
            // Add CanvasScaler
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create Game Over Panel
        if (gameOverPanel == null)
        {
            gameOverPanel = CreateGameOverPanel();
        }
        
        Debug.Log("Game Over UI setup complete! Remember to assign references in GameManager and GameOverUI scripts.");
    }
    
    private GameObject CreateGameOverPanel()
    {
        // Create Panel
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(gameOverCanvas.transform, false);
        
        // Add RectTransform and make it fill the screen
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Add Image component for background
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        
        // Add CanvasGroup for fade effects
        panel.AddComponent<CanvasGroup>();
        
        // Initially disable the panel
        panel.SetActive(false);
        
        // Create UI elements
        CreateGameOverTitle(panel);
        CreateRemainingYarnsText(panel);
        CreateButtons(panel);
        
        return panel;
    }
    
    private void CreateGameOverTitle(GameObject parent)
    {
        GameObject titleGO = new GameObject("GameOverTitle");
        titleGO.transform.SetParent(parent.transform, false);
        
        RectTransform rect = titleGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.8f);
        rect.anchorMax = new Vector2(0.5f, 0.8f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(400, 60);
        
        TextMeshProUGUI text = titleGO.AddComponent<TextMeshProUGUI>();
        text.text = "Game Over!";
        text.fontSize = 48;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
    }
    
    
    private void CreateRemainingYarnsText(GameObject parent)
    {
        GameObject yarnsGO = new GameObject("RemainingYarnsText");
        yarnsGO.transform.SetParent(parent.transform, false);
        
        RectTransform rect = yarnsGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.6f);
        rect.anchorMax = new Vector2(0.5f, 0.6f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(300, 40);
        
        TextMeshProUGUI text = yarnsGO.AddComponent<TextMeshProUGUI>();
        text.text = "Yarns Remaining: 0";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
    }
    
    private void CreateButtons(GameObject parent)
    {
        // Restart Button
        CreateButton(parent, "RestartButton", "Restart Game", new Vector2(0.5f, 0.25f));
        
        // Main Menu Button
        CreateButton(parent, "MainMenuButton", "Main Menu", new Vector2(0.5f, 0.15f));
        
        // Quit Button
        CreateButton(parent, "QuitButton", "Quit Game", new Vector2(0.5f, 0.05f));
    }
    
    private void CreateButton(GameObject parent, string name, string text, Vector2 anchorPosition)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent.transform, false);
        
        RectTransform rect = buttonGO.AddComponent<RectTransform>();
        rect.anchorMin = anchorPosition;
        rect.anchorMax = anchorPosition;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(200, 50);
        
        // Add Button component
        Button button = buttonGO.AddComponent<Button>();
        
        // Add Image component for button background
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create text child
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI buttonText = textGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        // Initially disable the button
        buttonGO.SetActive(false);
    }
}
