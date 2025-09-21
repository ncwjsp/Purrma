# 🐱 Purrma

A colorful and strategic puzzle game where you help a cat shoot yarn balls to create matching chains!

## 📖 Description

**Purrma** is a 2D puzzle game built in Unity that combines strategy, timing, and color-matching mechanics. Players control a cute cat character that shoots different colored yarn balls into a moving chain. The goal is to create matches of 3 or more consecutive yarn balls of the same color to clear them from the chain.

### 🎮 Game Features

- **Three Difficulty Levels**: Stage 1 (20 yarns), Stage 2 (25 yarns), and Stage 3 (30 yarns)
- **Strategic Gameplay**: Plan your shots carefully to create optimal matches
- **Color-Matching Mechanics**: Match 3+ consecutive yarn balls of the same color
- **Progressive Difficulty**: Each stage offers unique patterns and challenges
- **Smooth Animations**: Beautiful gap-closing animations when matches are made
- **Pause System**: Full pause functionality with UI integration

### 🎯 How to Play

1. **Aim**: Move your mouse to aim the cat's yarn ball
2. **Shoot**: Click to fire the yarn ball into the moving chain
3. **Match**: Create groups of 3+ consecutive yarn balls of the same color
4. **Clear**: Matched yarn balls disappear with satisfying animations
5. **Survive**: Prevent the chain from reaching the end of the path

## 🖼️ Screenshots

_Screenshots will be added here showing:_

- Main menu interface
  <img width="1280" height="720" alt="image" src="https://github.com/user-attachments/assets/ebc32b10-31b7-4c8c-94a7-f94fce0b43be" />

- Gameplay with different stages
  <img width="1280" height="720" alt="image" src="https://github.com/user-attachments/assets/2e9e1d4a-167e-4bbb-b721-7430284a1e33" />

- Stage 1
  <img width="1280" height="720" alt="image" src="https://github.com/user-attachments/assets/1354c0bf-1575-46fa-a2f2-feb2cfbd5820" />

- Stage 2
  <img width="1280" height="720" alt="image" src="https://github.com/user-attachments/assets/dce8c648-f5c1-49d8-a55d-f2e8550bd857" />

- Stage 3
  <img width="1280" height="720" alt="image" src="https://github.com/user-attachments/assets/2e21d8be-6ac3-450f-b3ab-f4fca336dd25" />

- Pause menu system
  <img width="1280" height="720" alt="image" src="https://github.com/user-attachments/assets/99c26f5d-4eee-480e-b3b1-46af74cac1b0" />

## 🛠️ Technical Details

- **Engine**: Unity 2022.3.62f1
- **Platform**: PC (Windows)
- **Graphics**: 2D with Universal Render Pipeline
- **Input System**: Unity's New Input System
- **Audio**: Unity Audio System with multiple sound effects
- **UI**: Unity uGUI with TextMeshPro

### 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Game/           # Core game mechanics
│   ├── Difficulty/     # Difficulty selection
│   └── Main Menu/      # Menu systems
├── Scenes/             # Game scenes (Stage 1, Stage 2, Stage 3, Main Menu)
├── Sprites/            # Game artwork and UI elements
├── Audio/              # Background music and sound effects
├── Fonts/              # Custom fonts for UI
└── Prefabs/            # Reusable game objects
```

## 🎮 Controls

- **Mouse Movement**: Aim the cat's yarn ball
- **Left Click**: Shoot yarn ball
- **Escape**: Pause/Resume game
- **UI Buttons**: Navigate menus and options

## 🚀 Getting Started

### Prerequisites

- Unity 2022.3.62f1 or later
- Windows 10/11 (for building)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/ncwjsp/Purrma.git
   ```

2. Open the project in Unity 2022.3.62f1

3. Open the scene `Assets/Scenes/Main Menu.unity`

4. Press Play to start the game

### Building

1. Go to `File > Build Settings`
2. Add the scenes you want to include
3. Select your target platform
4. Click `Build` to create the executable

## 🎯 Game Design

### Stage Progression

- **Stage 1**: 20 yarn balls, simple color introduction pattern
- **Stage 2**: 25 yarn balls, strategic almost-matches for planning
- **Stage 3**: 30 yarn balls, complex patterns with multiple match opportunities

### Core Mechanics

- **Yarn Chain System**: Moving chain of colored yarn balls
- **Shooting System**: Precise aiming and firing mechanics
- **Matching System**: 3+ consecutive same-color yarn balls create matches
- **Gap Animation**: Smooth animations when gaps are created
- **Game Over**: Chain reaches the end of the path

## 👥 Development Team

- Nueachai Wijitsopon 6510449
- Phonvan Deelertpattana 6610607
- Thananya Amornwiriya 6610609

## 🙏 Acknowledgments

- Unity Technologies for the game engine
- TextMeshPro for enhanced text rendering
- All contributors and testers who helped improve the game

## 🔊 Audio Credits

This game uses free sound effects from various sources. All audio assets are used in compliance with their respective licenses:

### Sound Effects

- **Background Music**: Free royalty-free music from various sources
- **UI Sounds**: Free sound effects from multiple open source libraries
- **Gameplay SFX**: Creative Commons licensed audio assets

---

**Made with ❤️ and Unity**
