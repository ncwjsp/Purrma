# Pause System Setup Guide

This guide will help you set up the pause system in your Unity game. The pause system includes a pause button, pause menu, and the ability to pause/resume, restart, and go to the main menu.

## Files Created

1. **PauseManager.cs** - Handles pause logic and time scale management
2. **PauseUI.cs** - Manages pause menu UI elements and button interactions
3. **Updated GameManager.cs** - Integrated with pause system

## Setup Instructions

### 1. Create Pause UI in Unity

1. **Create Pause Panel:**

   - Right-click in Hierarchy → UI → Panel
   - Name it "PausePanel"
   - Set it as a child of your main Canvas
   - Set its position to cover the entire screen
   - Set its color to semi-transparent black (0, 0, 0, 0.5)
   - Initially set it as inactive

2. **Add Pause Title:**

   - Right-click PausePanel → UI → Text - TextMeshPro
   - Name it "PauseTitle"
   - Set text to "Game Paused"
   - Center it at the top of the panel
   - Style as desired

3. **Add Pause Buttons:**

   - Right-click PausePanel → UI → Button - TextMeshPro
   - Create three buttons:
     - "ResumeButton" (text: "Resume")
     - "RestartButton" (text: "Restart")
     - "MainMenuButton" (text: "Main Menu")
   - Arrange them vertically in the center of the panel
   - Style as desired

4. **Create Pause Button:**
   - Right-click Canvas → UI → Button - TextMeshPro
   - Name it "PauseButton"
   - Position it in the top-right corner of the screen
   - Set text to "Pause" or use a pause icon
   - Style as desired

### 2. Add Scripts to GameObjects

1. **Create PauseManager GameObject:**

   - Right-click in Hierarchy → Create Empty
   - Name it "PauseManager"
   - Add the `PauseManager` script component
   - Assign the pause panel and pause button in the inspector

2. **Create PauseUI GameObject:**

   - Right-click in Hierarchy → Create Empty
   - Name it "PauseUI"
   - Add the `PauseUI` script component
   - Assign all UI elements in the inspector

3. **Update GameManager:**
   - Select your GameManager GameObject
   - In the Pause System section, assign:
     - PauseManager: Reference to the PauseManager GameObject
     - PauseUI: Reference to the PauseUI GameObject

### 3. Configure PauseManager Settings

In the PauseManager component:

- **Pause Key**: Set to Escape (default)
- **Can Pause**: Checked (default)
- **Pause Panel**: Assign the PausePanel GameObject
- **Pause Button**: Assign the PauseButton GameObject
- **Pause Sound**: Assign an AudioSource for pause sound (optional)
- **Resume Sound**: Assign an AudioSource for resume sound (optional)

### 4. Configure PauseUI Settings

In the PauseUI component:

- **Pause Panel**: Assign the PausePanel GameObject
- **Pause Title**: Assign the PauseTitle TextMeshPro component
- **Resume Button**: Assign the ResumeButton
- **Restart Button**: Assign the RestartButton
- **Main Menu Button**: Assign the MainMenuButton
- **Pause Button**: Assign the PauseButton
- **Pause Button Text**: Assign the PauseButton's TextMeshPro component
- **Button Click Sound**: Assign an AudioSource for button sounds (optional)

### 5. Audio Setup (Optional)

1. **Create Audio Sources:**

   - Add AudioSource components to the PauseManager GameObject
   - Assign pause and resume sound clips
   - Add AudioSource component to PauseUI GameObject for button sounds

2. **Sound Clips:**
   - Import pause/resume sound effects
   - Import button click sound effects
   - Assign them to the respective AudioSource components

## Features

### Pause Functionality

- **Escape Key**: Press Escape to pause/resume
- **Pause Button**: Click the pause button to pause
- **Time Scale**: Game time is paused (Time.timeScale = 0)
- **UI Management**: Pause panel appears, pause button hides

### Pause Menu Options

- **Resume**: Continue the game from where you left off
- **Restart**: Restart the current level
- **Main Menu**: Return to the main menu

### Integration

- **Game State Awareness**: Won't pause during game over or win states
- **GameManager Integration**: Works with existing game state management
- **Event System**: Uses events for clean communication between components

## Usage

1. **Pausing**: Press Escape or click the pause button
2. **Resuming**: Click Resume in the pause menu or press Escape again
3. **Restarting**: Click Restart in the pause menu
4. **Main Menu**: Click Main Menu in the pause menu

## Customization

### Changing Pause Key

- In PauseManager, change the `pauseKey` variable
- Common alternatives: KeyCode.P, KeyCode.Space, etc.

### Disabling Pause

- Uncheck "Can Pause" in PauseManager
- Or call `pauseManager.SetCanPause(false)` in code

### UI Styling

- Modify the pause panel appearance
- Change button styles and colors
- Add animations for smooth transitions

### Additional Features

- Add settings menu in pause panel
- Add volume controls
- Add level selection
- Add save/load functionality

## Troubleshooting

### Common Issues

1. **Pause not working**: Check if PauseManager is assigned in GameManager
2. **UI not showing**: Verify all UI elements are assigned in PauseUI
3. **Buttons not working**: Check if button listeners are properly set up
4. **Time not pausing**: Ensure Time.timeScale is being set correctly

### Debug Tips

- Check console for error messages
- Verify all references are assigned in the inspector
- Test pause functionality in play mode
- Check if other scripts are interfering with time scale

## Notes

- The pause system automatically prevents pausing during game over or win states
- Time scale is properly restored when the game object is destroyed
- The system is designed to work with your existing GameManager structure
- All pause-related functionality is contained in the PauseManager and PauseUI scripts
