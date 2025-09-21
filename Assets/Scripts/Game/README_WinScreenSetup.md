# Win Screen Setup Guide

This guide explains how to set up the win screen system in your Unity project.

## Components Added

1. **YarnChainManager.cs** - Enhanced with win condition detection
2. **GameManager.cs** - Updated to handle win state
3. **WinUI.cs** - UI controller for win screen
4. **Player scripts** - Updated to block input during win state

## Setup Instructions

### 1. Create Win Screen UI Canvas

1. Create a new Canvas in your scene (right-click in Hierarchy > UI > Canvas)
2. Name it "WinCanvas"
3. Set it to "Screen Space - Overlay"
4. Add a CanvasGroup component to the root Canvas

### 2. Create Win Panel

1. Right-click on WinCanvas > UI > Panel
2. Name it "WinPanel"
3. Set the panel to fill the entire screen
4. Set the background color to semi-transparent green (0, 0.5, 0, 0.8)
5. Initially disable this panel

### 3. Add UI Elements to Win Panel

Add these UI elements as children of WinPanel:

#### Win Title

- Right-click WinPanel > UI > Text - TextMeshPro
- Name: "WinTitle"
- Text: "You Win!"
- Font Size: 48
- Color: White
- Center alignment

#### Win Message

- Right-click WinPanel > UI > Text - TextMeshPro
- Name: "WinMessage"
- Text: "Congratulations! You cleared all the yarns!"
- Font Size: 24
- Color: White
- Center alignment

#### Buttons

Create these buttons as children of WinPanel:

1. **Next Level Button**

   - Right-click WinPanel > UI > Button
   - Name: "NextLevelButton"
   - Text: "Next Level"
   - Initially disable this button

2. **Main Menu Button**

   - Right-click WinPanel > UI > Button
   - Name: "WinMainMenuButton"
   - Text: "Main Menu"
   - Initially disable this button

3. **Quit Button** (optional)
   - Right-click WinPanel > UI > Button
   - Name: "QuitButton"
   - Text: "Quit Game"
   - Initially disable this button

### 4. Create Win UI GameObject

1. Create an empty GameObject in your scene
2. Name it "WinUI"
3. Add the WinUI script
4. Assign the UI references in the inspector:
   - Win Panel: Drag WinPanel
   - Win Title: Drag WinTitle
   - Win Message: Drag WinMessage
   - Next Level Button: Drag NextLevelButton
   - Main Menu Button: Drag WinMainMenuButton
   - Quit Button: Drag QuitButton (if created)

### 5. Update GameManager

1. Select your GameManager GameObject
2. In the Inspector, find the "Win UI" section
3. Assign the UI references:
   - Win Panel: Drag WinPanel
   - Win Text: Drag WinTitle
   - Next Level Button: Drag NextLevelButton
   - Win Main Menu Button: Drag WinMainMenuButton

### 6. Add Audio Sources (Optional)

1. Add AudioSource components to WinUI for:
   - Win Sound
   - Button Click Sound
2. Assign your win sound audio clip

## How It Works

1. **Win Detection**: When all yarns are cleared from the chain, the win condition is triggered
2. **UI Display**: The win screen fades in with congratulations message
3. **Player Actions**: Players can go to the next level or return to the main menu
4. **Input Blocking**: Player input is completely disabled during win state

## Win Conditions

- **Primary**: All yarns in the chain are cleared (yarns.Count == 0)
- **Secondary**: Game is not already over (prevents win after game over)

## Features

- **Smooth Animations**: Fade-in effects for the win screen
- **Multiple Actions**: Next level, main menu, or quit options
- **Audio Integration**: Support for win sounds and button clicks
- **Input Blocking**: Complete player input prevention during win state
- **Stage Progression**: Automatic progression to next stage when "Next Level" is clicked

## Testing

1. Play the game
2. Clear all yarns by making matches
3. Verify the win screen appears
4. Test next level and main menu buttons
5. Check that player input is blocked during win state

## Customization

- Adjust win screen colors and animations in WinUI.cs
- Modify win message text in the WinMessage TextMeshPro component
- Add more win statistics or achievements as needed
- Customize button layouts and styling
