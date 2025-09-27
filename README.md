<p align="center">
  <img src="https://github.com/user-attachments/assets/89f9c599-7cba-4183-a50a-4780173578c6" alt="Image 2" width="250">
</p>

Icy Tower-Inspired Platformer

A 2D vertical platformer prototype built in Unity (C#), inspired by Icy Tower.  
This project was developed during the QueenB Bootcamp as part of a team of 3 developers, with most of the work completed in a 5-day sprint.  
The bootcamp focused on mobile game development, and since we didn't find many mobile implementations of the nostalgic Icy Tower we all loved, we chose to create a mobile-friendly version.  
It is also our first experience with Unity, making this both a learning journey and an opportunity to refine our skills in game development and object-oriented design.

/// Gameplay

* The player jumps upward on endless platforms while the camera scrolls with them.  
* Falling below the camera ends the game.  
* Score is earned by:  
  Climbing to higher floors.  
  Achieving combo jumps that skip multiple platforms at once.  
* Platform variety adds depth and unpredictability:  
  Normal platforms  
  Moving platforms  
  Sticky platforms  
  Bouncy platforms  
(more types planned)  
* Power-up system introduces temporary abilities such as:  
Jetpack- sustained upward thrust with reduced gravity.  
Speed Boost- increased horizontal acceleration and visual feedback.  
* The camera speeds up the higher you climb, increasing difficulty over time.  
* Every 10th platform is marked with the floor number, visualizing player progression.
* Background music is played and sound effects are triggered on jumps and progression.  
* Main Menu and game instructions are featured on game start.

We created an extensible input provider interface and kept the game logic generic, so the game can easily be extended to PC and other platforms.

/// Design   

This project was also an exercise in clean code architecture and object-oriented programming, and we tried to follow core principles:  

Interfaces for extensibility:
- IAbility defines how new power-ups behave.
- InterfacePlayerInput abstracts away input sources (keyboard, touch, etc).

Abstract base classes
- PowerUpPickup handles collision detection and ability application, letting derived classes only define the specific ability.

Events & decoupling
- SpawnManager emits OnPlatformSpawned events, which power-up spawners or other systems can subscribe to without tight coupling.

Pooling
- Instead of destroying/instantiating objects repeatedly, platforms and boards are reused to improve performance and stability.

Class Structure (simplified)   
PlayerController - Handles movement, physics, wall bouncing, and applying abilities.  
SpawnManager - Spawns endless platforms and walls, tracks floor count, and spawns floor boards.  
PowerUpPickup (abstract) - Generic pickup behavior, linked to:  
  JetpackPickup  
  SpeedBoostPickup  
IAbility (interface) - Defines Activate and Deactivate, implemented by:  
  JetpackAbility  
  SpeedBoostAbility  

Project Status
- Core gameplay loop functional (movement, spawning, scoring, power-ups).
- UI and overall look is being polished.
- Improvements to player movement and jumps ongoing.

Next steps: animations, scoreboard, level progression, general polish, and extended platform/power-up types.

<p align="center">
    <img src="https://github.com/user-attachments/assets/5636648a-8c13-4651-877a-7555197c0fb2" alt="Image" width="250">
  <img src="https://github.com/user-attachments/assets/170733e1-08e9-44dd-9ba7-29ed4bc0d51d" alt="Image 1" width="250">
  <img src="https://github.com/user-attachments/assets/0ca07f3b-2c21-4b8d-a094-89a3aaa4a116" alt="Image 3" width="250">
</p>
