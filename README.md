# Game Design Document (GDD)

## 1. Game Title
**Project:** Urban Courier (Codename)

---

## 2. Concept Summary (Elevator Pitch)
A fast-paced 3D third-person parkour game where players control an agile courier, utilizing fluid root-motion animations to dynamically vault over obstacles while collecting highly sensitive data tokens scattered across the city rooftops.

---

## 3. Genre and Platform
- **Genre:** 3D Platformer, Parkour Action, Collectathon  
- **Platform:** PC and Console (Full Gamepad and Keyboard/Mouse support)

---

## 4. Target Audience
Fans of momentum-based movement games, speedrunners, and players who appreciate smooth, physically grounded character controllers and satisfying collectible loops.

---

## 5. Developers
- **Student Number:** 2400008056  
- **First Name:** Qasım  
- **Last Name:** Kazmı  

---

# Mechanics and Gameplay

## 1. Core Gameplay Loop
- **Traverse:**  
  The player navigates a 3D environment, using momentum to approach various physical barriers.

- **Detect & Vault:**  
  A dynamic 2-step raycast detection system scans for obstacles and calculates the exact height and depth of the wall.

- **Overcome:**  
  The player vaults over obstacles using mathematical alignment and Target Matching to ensure smooth interaction without clipping.

- **Collect:**  
  The player intercepts **Data Tokens**, triggering a decoupled event-driven system that updates the score.

---

## 2. Controls & Character Control Architecture
The character controller is entirely physics and animation-driven:

- **Movement (Left Stick / WASD):**  
  Driven by Root Motion via an Animator Blend Tree. Movement values are passed into the animator to sync animation and physics perfectly.

- **Parkour / Jump (South Button / Spacebar):**  
  When near a valid obstacle, the system bypasses standard jump physics and triggers a vault animation using:
  ```csharp
  animator.CrossFadeInFixedTime("Vault", 0.15f);
