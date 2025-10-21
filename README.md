# 🌌 RealMethod
*A lightweight Unity architecture for scalable and modular project creation.*
---

## ✨ Overview  

**RealMethod** is a Unity package that defines a clean and consistent **project architecture**.  
It focuses on separation of responsibility and lifecycle management, allowing developers to build games that are easily extendable, maintainable, and modular.  

RealMethod introduces five main layers:  
- **Core** – Defines the base relationships between Game, World, Manager, and Service.  
- **Library** – Common utility functions, interfaces, and shared components.  
- **Pattern** – Predefined architecture patterns and reusable managers.  
- **Toolkit** – Ready-to-use tools, inspectors, and debugging helpers.  
- **ReadySet** – Example setups and templates for quick project initialization.  

---

## 🧩 Core Architecture  

### **Game**
- The **Game** class is the entry point of the RealMethod framework.  
- It is a **singleton** that lives for the entire game lifecycle (until quit).  
- Responsible for global operations such as:  
  - Scene management (`OpenScene`, `ReloadScene`, etc.)  
  - Time control (`SetGameSpeed`)  
  - Service construction and destruction  

All **Managers** and **Services** are registered through `Game`, ensuring consistent global access.  

---

### **World**
- The **World** class exists **once per scene** and acts as the runtime context for that scene.  
- It initializes all **World-scoped Managers** and prepares the scene for gameplay.  
- While **Game** persists between scenes, **World** resets when scenes change.  
- Required in every scene that uses RealMethod.  

---

### **Manager**
- A **Manager** defines a controllable system within a given scope (Game or World).  
- Implements the `IManager` interface.  
- Managers are initialized **before Unity’s Awake/Start** callbacks.  
- Can live in one of two scopes:  
  - **Game scope:** persists between scenes (registered under Game).  
  - **World scope:** tied to a specific scene (registered under World).  

Both Game and World automatically call `DontDestroyOnLoad` for their scoped Managers.  

---

### **Service**
- A **Service** is a lightweight, non-MonoBehaviour object managed by the Game.  
- Created dynamically using `Game.CreateService<T>()`.  
- All Managers automatically recognize and can interact with new Services.  
- Can be destroyed at any time via `Game.DestroyService<T>()`.  
- Ideal for runtime systems like data caching, analytics, or network clients.  

---

## ⚙️ Lifecycle Summary  

| Layer | Scope | Lifetime | Created By | Notes |
|-------|--------|-----------|-------------|-------|
| Game | Global | Until Quit | App Start | Singleton |
| World | Scene | Per Scene | Scene Load | Scene Context |
| Manager | Global/Scene | Anytime | Game/World | Registered in Game/Scene scope |
| Service | Runtime | Dynamic | Game | Fully managed objects |

---

## 📦 Package Structure  
RealMethod is organized into clearly separated runtime, editor, and resource layers — each designed to keep your project scalable, readable, and modular.
### 🧩 Runtime
Core runtime code that runs inside Unity builds.

- **Core/**
  - `Architecture/` – Core systems such as **Game**, **World**, **Manager**, and **Service**.  
  - `Attributes/` – Custom attributes used across the framework.  
  - `Definitions/` – Global enums, tags, and static definitions.  
  - `ProjectSetting/` – Runtime configuration and project constants.  

- **Library/**
  - `Extension/` – Unity extensions and helper methods.  
  - `Interfaces/` – Common interfaces for modular communication.  
  - `SharedScripts/` – Shared data structures (Classes, Enums, Structs).  
  - `Utilities/` – Core utility scripts.  
  - `Vendor/SerializableDictionary/` – Third-party generic dictionary support.  

- **Pattern/**
  - `Components/` – Base components using RealMethod patterns.  
  - `DataAssets/` – ScriptableObject-based configuration.  
  - `DesignPatterns/` – Common gameplay and architecture patterns.  
  - `Managers/` – Runtime manager implementations.  
  - `Services/` – Service classes managed by the Game/World.  

- **ReadySet/**
  - `Commands/` – Command execution framework (Executors, Tasks).  
  - `Components/` – Common building blocks (Input, Method, Physics, Time, UI, Visual).  
  - `DefaultsClass/` – Predefined data and logic templates.  
  - `Managers/` – Ready-to-use manager classes.  
  - `Presets/` – Resource and pooling presets (PoolAsset, ResourceAsset, Task).  
  - `Services/` – Prebuilt services and systems.  

- **Toolkit/**
  - `Ability/` – Ability system and samples.  
  - `Actor/` – Actor handling and lifecycle logic.  
  - `CSVFile/` – CSV parsing and data import tools.  
  - `CurveViewer/` – Editor curve visualization.  
  - `Interaction/` – Interaction handling system.  
  - `Inventory/` – Inventory architecture.  
  - `PCG/` – Procedural generation tools and resources.  
  - `Pickup/` – Item pickup logic.  
  - `RPG/` – RPG systems (Resource, StatSystem).  
  - `Tutorial/` – Tutorial system and utilities.  
  - `Upgrade/` – Upgrade and progression tools.  

---

### 🛠️ Editor
Unity Editor extensions for all RealMethod layers.

- **Core/** – Editor scripts for Architecture, Definitions, and Project Settings.  
- **Library/** – Shared scripts, vendor tools, and utilities.  
- **Pattern/** – Custom editors for data assets and managers.  
- **ReadySet/** – Editor tools for content setup and asset generation.  
- **Toolkit/** – Editors for gameplay systems (Ability, Inventory, TerrainTools, etc.).  

---

### 🧃 Reservoir
Central storage for framework assets.

- `Icons/` – Organized icons for Core, Pattern, and Toolkit editors.  
- `Prefabs/` – Prefab resources for samples and managers.  
- `SceneTemplates/` – Example scenes for quick setup.  
- `ScriptTemplates/` – Script templates for fast development.  

---

### 🧪 Samples~
Example projects demonstrating **PopupMessage** and **Tutorial** systems.

---

### 🧠 Tests
Unit and integration tests for **General**, **Inspector**, and **ScenesDot** systems.
