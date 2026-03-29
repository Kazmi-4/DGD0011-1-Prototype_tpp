# 📑 PROJECT DOCUMENTATION: URBAN COURIER
**Technical Implementation & Scriptable Object Architecture**

---

## 1. Executive Summary
**Urban Courier** is a high-fidelity 3D third-person parkour prototype. Unlike standard "capsule-based" controllers, this project implements a professional **Root Motion** system where physical displacement is driven directly by animation data. The backend utilizes a strictly decoupled **Scriptable Object (SO) Architecture** to ensure modularity, scalability, and high performance.

---

## 2. Core Game Mechanics

### 2.1 Animation-Driven Locomotion (Root Motion)
The character controller is built to avoid "foot sliding" and "moonwalking."
* **Implementation:** The `ThirdPersonController.cs` utilizes the `ReceiveRootMotion` callback. 
* **Logic:** Unity’s Animator calculates the precise distance the character's feet move in 3D space. This delta is passed to the `CharacterController.Move()` function, ensuring the physical capsule stays perfectly synced with the 3D mesh.
* **Blend Trees:** A **2D Simple Directional Blend Tree** handles the transition between Idle, Walk, and Sprint based on `InputX` and `InputY` parameters.

### 2.2 Procedural Vaulting & Target Matching
The vaulting system is not a static "canned" animation; it is procedural and height-aware.
* **Dual-Step Raycast Detection:**
    1. **Forward Ray:** Scans for obstacles within a `1.2m` range.
    2. **Downward Ray:** Shoots from `vaultMaxHeight` down to find the exact Y-coordinate of the ledge.
* **Target Matching:** Using `animator.MatchTarget()`, the system mathematically "stretches" the Vault animation so the character’s **Right Hand** snaps perfectly to the ledge's edge, regardless of the obstacle's specific height.
* **Code-Driven Transitions:** Vaulting is triggered via `animator.CrossFadeInFixedTime("Vault", 0.15f)`, bypassing the need for messy transition arrows in the Animator window.

---

## 3. Scriptable Object Architecture
The project fulfills the assignment's modularity requirements by using SOs to decouple game systems.

### 3.1 Variables & Flexible References
* **Decoupling:** Player stats (Score) are stored in `FloatVariable` assets.
* **Flexible Reference Struct:** The `FloatReference` allows designers to toggle between a **Constant** value or a **Variable** asset in the Inspector. This allows for global data sharing without hardcoding script dependencies.

### 3.2 Game Event System (Broadcaster/Listener)
Communication between "Data Tokens" (collectibles) and the UI is handled via a **Broadcaster/Listener** pattern.
* **The Broadcaster:** When a token is collected, it raises a `GameEvent` SO (`Event_ScoreChanged`).
* **The Listener:** The UI `GameEventListener` component hears this signal and triggers the `UpdateScoreText` function.
* **Benefit:** The token script does not know the UI exists, and the UI does not know the token exists. They are 100% independent.

### 3.3 Runtime Sets
* **Dynamic Tracking:** A `CollectibleSet` (derived from `RuntimeSet<T>`) tracks all active tokens in the level.
* **Performance:** Objects automatically add/remove themselves from the set on `OnEnable`/`OnDisable`. This eliminates the need for expensive `FindObjectsOfType` calls during gameplay.

### 3.4 Advanced Enums (Data Containers)
* **Design:** Item types (e.g., `Type_Coin`) are created as `ItemTypeSO` assets. This allows designers to add new item types to the project without writing a single line of C# code.

---

## 4. Build & Stability Optimizations
Extensive debugging was performed to ensure the project remains stable in a standalone `.exe` build.

### 4.1 Framerate Normalization
* **Problem:** In standalone builds, high framerates (1000+ FPS) caused desync between the Animator’s `normalizedTime` and C# logic, leading to character freezing.
* **Solution:** Implemented `Application.targetFrameRate = 60;` to normalize physics and animation timing across all hardware.

### 4.2 Transition Locking
* **Problem:** Calling `MatchTarget` while the Animator is still "blending" (transitioning) into a state causes a fatal error in the Unity Animation Engine.
* **Solution:** Wrapped all snapping logic in a strict check: `if (!animator.IsInTransition(0))`. This ensures the hand only snaps once the Vault state is fully active.

### 4.3 Shader & Material Compatibility
* **Problem:** ProBuilder's default "Vertex Color" shaders are incompatible with standard build compilers.
* **Solution:** All level geometry was stripped of ProBuilder materials and replaced with **Standard URP Materials** to ensure a 100% successful build process.

---

## 5. UI Implementation
* **TextMeshPro (TMP):** High-fidelity text rendering used for all score displays.
* **Anchor Presets:** UI elements are anchored using **Top-Center** presets to maintain layout consistency across various screen aspect ratios.

---

## 📂 Project Folder Hierarchy
* `_Project/Scripts/`: Core C# logic (Controller, SO Templates, Event System).
* `_Project/ScriptableObjects/`: Data files (Events, Variables, Runtime Sets, Item Types).
* `_Project/Prefabs/`: Character and Interactive Object templates.
* `_Project/Scenes/`: Main Level and Demonstration environment.

---

## 📝 Developer Information
* **Developer Name:** Qasim Kazmi
* **Student Number:** 240008056
* **Course:** DGD0011-1 - Homework 1
