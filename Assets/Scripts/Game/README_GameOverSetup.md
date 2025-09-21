# Game Over System Setup Guide

This guide explains how to set up the game over system in your Unity project.

## Components Added

1. **YarnChainManager.cs** - Enhanced with game over detection
2. **GameManager.cs** - Main game state manager
3. **GameOverUI.cs** - UI controller for game over screen

## Setup Instructions

### 1. Create Game Over UI Canvas

1. Create a new Canvas in your scene (right-click in Hierarchy > UI > Canvas)
2. Name it "GameOverCanvas"
3. Set it to "Screen Space - Overlay"
4. Add a CanvasGroup component to the root Canvas

### 2. Create Game Over Panel

1. Right-click on GameOverCanvas > UI > Panel
2. Name it "GameOverPanel"
3. Set the panel to fill the entire screen
4. Set the background color to semi-transparent black (0, 0, 0, 0.8)
5. Initially disable this panel

### 3. Add UI Elements to Game Over Panel

Add these UI elements as children of GameOverPanel:

#### Game Over Title

- Right-click GameOverPanel > UI > Text
- Name: "GameOverTitle"
- Text: "Game Over!"
- Font Size: 48
- Color: White
- Center alignment

#### Remaining Yarns Text

- Right-click GameOverPanel > UI > Text
- Name: "RemainingYarnsText"
- Text: "Yarns Remaining: 0"
- Font Size: 24
- Color: White
- Center alignment

#### Buttons

Create these buttons as children of GameOverPanel:

1. **Restart Button**

   - Right-click GameOverPanel > UI > Button
   - Name: "RestartButton"
   - Text: "Restart Game"
   - Initially disable this button

2. **Main Menu Button**

   - Right-click GameOverPanel > UI > Button
   - Name: "MainMenuButton"
   - Text: "Main Menu"
   - Initially disable this button

3. **Quit Button** (optional)
   - Right-click GameOverPanel > UI > Button
   - Name: "QuitButton"
   - Text: "Quit Game"
   - Initially disable this button

### 4. Create Game Manager GameObject

1. Create an empty GameObject in your scene
2. Name it "GameManager"
3. Add the GameManager script
4. Assign the UI references in the inspector:
   - Game Over Panel: Drag GameOverPanel
   - Game Over Text: Drag GameOverTitle
   - Remaining Yarns Text: Drag RemainingYarnsText
   - Restart Button: Drag RestartButton
   - Main Menu Button: Drag MainMenuButton

### 5. Create Game Over UI GameObject

1. Create an empty GameObject in your scene
2. Name it "GameOverUI"
3. Add the GameOverUI script
4. Assign the UI references in the inspector:
   - Game Over Panel: Drag GameOverPanel
   - Game Over Title: Drag GameOverTitle
   - Remaining Yarns Text: Drag RemainingYarnsText
   - Restart Button: Drag RestartButton
   - Main Menu Button: Drag MainMenuButton
   - Quit Button: Drag QuitButton (if created)

### 6. Configure YarnChainManager

1. Select your YarnChainManager GameObject
2. In the inspector, find the "Game Over Settings" section
3. Enable "Game Over Enabled"
4. Set "Game Over Threshold" to 0.95 (95% of the path)

### 7. Add Audio Sources (Optional)

1. Add AudioSource components to GameManager for:
   - Game Over Sound
   - Background Music
2. Add AudioSource component to GameOverUI for:
   - Button Click Sound

## How It Works

1. **Game Over Detection**: When any yarn reaches 95% of the spline path, the game over is triggered
2. **UI Display**: The game over screen fades in showing remaining yarns
3. **Player Actions**: Players can restart the game or return to the main menu

## Customization

- Adjust `gameOverThreshold` in YarnChainManager to change when game over triggers
- Customize UI appearance and animations in GameOverUI.cs
- Add more statistics or achievements as needed

## Testing

1. Play the game
2. Let yarns reach the end of the spline
3. Verify the game over screen appears
4. Test restart and main menu buttons
