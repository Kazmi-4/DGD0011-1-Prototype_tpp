Assignment Fulfillment Summary
This prototype successfully implements all required technical systems outlined in the assignment rubric:

Core Mechanics: Implemented a custom 3D character controller utilizing full Root Motion for locomotion, alongside a procedural vaulting mechanic (using Animator Target Matching) and a decouple "Data Token" collectible system.

Lighting: The scene utilizes the primary Directional Light set to Realtime for dynamic shadows, alongside a custom Point Light set to Baked mode, which was successfully calculated and baked onto the static level geometry.

Raycasting: Implemented a dual-step Raycast system to drive the vaulting logic. A forward ray scans for obstacle layers, while a secondary downward ray dynamically calculates the exact Y-coordinate of the ledge to accurately position the character's hands.

Camera Management: Integrated the Cinemachine package to manage three distinct Virtual Cameras (Standard Follow, Action Sprint, and Static Security/CCTV). A Custom Blends asset was created and assigned to the Cinemachine Brain to manage specific transition times and easing styles (e.g., Ease In Out, instant Cut) when toggling between camera priorities.

Visual Effects: Developed a custom Shader Graph (HoloTokenShader) for the URP. The graph utilizes a Fresnel Effect node multiplied by a high-intensity HDR Color node to create a dynamic, holographic glowing-edge effect, which was applied as a material to the in-game collectibles.





# 📑 PROJECT DOCUMENTATION: URBAN COURIER

**Developer Name:** Qasim Kazmi  
**Student Number:** 240008056  
**Course:** DGD0011-1 - Homework 1  
**Project Status:** Final Submission Ready  
**Architecture:** Decoupled Scriptable Object Pattern  
**Core Mechanic:** Root Motion & Procedural Target Matching  

---

## 1. Executive Summary & GDD

### 1.1 The Concept
**Urban Courier** is a high-fidelity 3D third-person parkour prototype. The project focuses on creating a professional-grade controller where animations drive the physics (Root Motion), rather than code forcing the character to slide. The player navigates an urban obstacle course to intercept "Data Tokens."

### 1.2 The Core Loop
* **Locomotion:** Navigating the environment using an animation-driven Blend Tree.
* **Obstacle Detection:** Dynamically scanning geometry via 2-step raycasting.
* **Vault Execution:** Using Target Matching to align the character’s hand to physical ledges.
* **Data Interception:** Collecting tokens to update a decoupled UI via Scriptable Object Game Events.

---

## 2. Character Control & Animation

### 2.1 Root Motion Implementation
Unlike standard controllers that move a "capsule" independently of the animation, this project uses a full Root Motion configuration to ensure zero "foot sliding."
* **Physics Sync:** The `ThirdPersonController.cs` script utilizes the `ReceiveRootMotion(Vector3 animationDeltaPosition)` callback.
* **The Logic:** Unity’s Animator calculates the precise distance the feet moved in the animation. This delta is passed to the `CharacterController.Move()` function, keeping the physical capsule perfectly synced with the 3D mesh.

### 2.2 Animator Architecture
* **Locomotion Blend Tree:** A **2D Simple Directional Blend Tree** handles smooth transitions between Idle, Walk, and Sprint based on `InputX` and `InputY` parameters.
* **Code-Driven Transitions:** To avoid "Spaghetti" Animator controllers, the system uses `animator.CrossFadeInFixedTime("Vault", 0.15f)`. This allows the script to trigger the vault state instantly with a smooth blend, bypassing manual transition lines.

### 2.3 Procedural Vaulting & Target Matching
The vaulting system is procedural and height-aware rather than a static animation.
* **Dual-Step Raycast Detection:**
    1.  **Forward Ray:** Scans for obstacles within a `1.2m` range.
    2.  **Downward Ray:** Shoots from `vaultMaxHeight` down to find the exact Y-coordinate of the ledge.
* **Target Matching:** Using `animator.MatchTarget()`, the system mathematically "stretches" the Vault animation so the character’s **Right Hand** snaps perfectly to the ledge's edge, regardless of the obstacle's specific height.

---

## 3. Scriptable Object Architecture
The project strictly follows the **Modular Unity Architecture** to ensure systems are independent (decoupled).

### 3.1 Variables & Flexible References
* **Decoupling:** Player stats (Score) are stored in `FloatVariable` assets in the Project folder.
* **Flexible Reference Struct:** The `FloatReference` allows designers to toggle between a **Constant** value or a **Variable** asset in the Inspector without touching code.

### 3.2 Game Event System (Broadcaster/Listener)
* **The Broadcaster:** When a token is collected, it raises a `GameEvent` Scriptable Object (`Event_ScoreChanged`).
* **The Listener:** The UI `GameEventListener` component hears this signal and triggers the `UpdateScoreText` function.
* **Benefit:** The token script does not know the UI exists, and vice versa. They are 100% independent.

### 3.3 Runtime Sets
* **Dynamic Tracking:** A `CollectibleSet` tracks all active tokens in the level. 
* **Performance:** Objects automatically register/unregister from the set on `OnEnable`/`OnDisable`. This eliminates the need for expensive `FindObjectsOfType` calls.

### 3.4 Advanced Enums
* **Design:** Item types (e.g., `Type_Coin`) are created as `ItemTypeSO` assets. This allows designers to add new item categories directly in the Project folder without writing new C# enums.

---

## 4. UI & Interaction System

### 4.1 TextMeshPro Implementation
* **Rendering:** TextMeshPro (TMP) is used for high-quality, resolution-independent text.
* **Anchor Presets:** UI elements are anchored using **Top-Center** presets to maintain layout consistency across various screen aspect ratios (16:9, 21:9, etc.).
* **Canvas Management:** The UI resides in a **Screen Space - Overlay** Canvas, ensuring it remains projected correctly on the player's screen at all times.

---

## 5. Troubleshooting & Build Stability Fixes
Extensive debugging was performed to resolve "Build-Only" bugs that do not appear in the Unity Editor.

### 5.1 The ProBuilder Shader Crash
* **Issue:** Standalone builds failed because ProBuilder's default "Vertex Color" shader is incompatible with standard compilers.
* **Fix:** All ProBuilder geometry was stripped of default materials and replaced with custom **URP Standard Materials**.

### 5.2 The 1000 FPS Animation Freeze
* **Issue:** At ultra-high framerates in builds, the Animator's `MatchTarget` math desynced, causing character freezing.
* **Fix:** Implemented `Application.targetFrameRate = 60;` in the `Awake()` method to normalize physics and animation timing.

### 5.3 MatchTarget Transition Lock
* **Issue:** Calling `MatchTarget` while the Animator is still "blending" into a state causes a fatal error.
* **Fix:** Wrapped snapping logic in a strict check: `if (stateInfo.IsName("Vault") && !animator.IsInTransition(0))`.

---

## 6. Project Folder Hierarchy
* **_Project/Scripts/:** Core C# logic (Controller, SO Templates, Event System).
* **_Project/ScriptableObjects/:** Data files (Events, Variables, Runtime Sets, Item Types).
* **_Project/Prefabs/:** Character and Interactive Object templates.
* **_Project/Scenes/:** Main Level and Demonstration environment.

---

## 7. Final Submission Checklist
* [✅] **GDD:** Complete and faithfully implemented.
* [✅] **Root Motion:** Configured and driving the Character Controller.
* [✅] **Target Matching:** Hands snap to ledges procedurally.
* [✅] **Animator Architecture:** Blend Trees + CrossFade logic used.
* [✅] **SO Variables:** Flexible references implemented.
* [✅] **SO Events:** Broadcaster/Listener pattern used for UI.
* [✅] **Runtime Sets:** Dynamic tracking of collectibles.
* [✅] **Advanced Enums:** ItemTypeSO used instead of hardcoded enums.
