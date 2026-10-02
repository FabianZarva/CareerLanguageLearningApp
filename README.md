# About the App

This app is an interactive educational mobile application prototype. It is designed to support contextual English vocabulary acquisition within realistic professional work-trial simulations. The experience personalizes onboarding, gameplay mechanics, and career navigation based on the user's Bartle gamer typologies and occupational interests.

This project served as the graduation assignment for the Creative Media and Game Technologies (CMGT) Bachelor programme at Hanze University of Applied Sciences Groningen.

---

## Core Features

* **Data-Driven Questionnaire and Profiling**:
  * Loads question data, category classifications, and scoring weights dynamically from XML resources.
  * Calculates profile weights for Bartle gamer types (Killer, Socializer, Achiever, Explorer) and career preferences.
  * Accommodates tied scores by retaining multiple dominant gamer types concurrently (`FinalGamerTypes`).
  * Features a three-tier job ranking system allowing players to establish first, second, and third career preferences.
  * Implements an undo and back-navigation stack (`answerHistory`) that restores the exact preceding score state when revisiting previous questions.

* **Eki Personal Assistant**:
  * Visual identity inspired by stickman illustrations from *The Henry Stickmin Collection*.
  * Displays dialogue via an animated typewriter system synchronized with typing audio feedback.
  * Switches dynamically between distinct character poses (`Idle`, `Point`, `Checklist`) mapped directly to specific dialogue lines.
  * Employs right-swipe touch gestures to advance dialogue, autocomplete typing animations, and navigate onboarding screens.

* **Career Selection Menu**:
  * Presents three distinct career paths: Lawyer, Cook, and Doctor/Surgeon.
  * Implements a mobile-friendly hold-to-confirm selection mechanic (1.2-second hold) to prevent accidental taps.
  * Delivers dynamic visual feedback during hold interactions via a radial fill indicator, card scaling, and background dimming of non-selected options.
  * Includes a confirmation safeguard popup before resetting saved progress and PlayerPrefs data when returning to the introduction menu.

* **Adaptive Menus by Gamer Type**:
  * **Non-Explorer Path**: Offers a direct, portrait-oriented menu presenting work-trials as a clean button list for immediate access.
  * **Explorer Path**: Renders 2D career environments to a `RenderTexture`, enabling free-form hold-to-move touch navigation, pinch-to-zoom camera controls, a directional arrow indicator, and movement sound effects.
  * **Dynamic Dialogue Assembly**: Generates contextual Eki dialogue tailored to the active profession and detected gamer types.

* **Achiever Scaffolding Progression**:
  * Replaces legacy in-game shop currencies with a task-based progression model.
  * Progressively unlocks work trials in sequence as preceding trials are completed, storing progress persistently in `PlayerPrefs`.
  * Synchronizes locked and unlocked states across both direct menu buttons and Explorer world access triggers.

* **Contextual Work-Trial Mini-Games**:
  * **Terms and Explanations** (Landscape): Matches definitions to terms, featuring individual representative icons for every term option to reinforce contextual vocabulary acquisition.
  * **Find the Object** (Landscape): Requires players to drag and drop written terminology cards directly onto matching environmental objects within the workplace scene.
  * **Find the -Nym** (Portrait): Evaluates semantic relations between words, supported by a vertically scrollable in-game encyclopedia containing detailed definitions and examples.
  * **Educational Feedback**: Displays informative explanations and hints for both correct and incorrect answers to foster continuous learning.

---

## Technical Specifications

* **Game Engine**: Unity 2022.3.62f2 (LTS) targeting Android.
* **Programming Language**: C#.
* **Testing Hardware**: Unity Device Simulator (Google Pixel 5) and physical testing on Google Pixel 6.
* **UI and Typography**: TextMeshPro utilizing the `LDF Comic Sans` font asset for readability and stylistic cohesion.
* **Audio Architecture**: Centralized `AudioManager` singleton (`DontDestroyOnLoad`) handling persistent background music, career focus tracks, and dedicated one-shot sound effects.
* **Dynamic Orientation**: Controlled per scene via `SceneOrientationController.cs`, transitioning seamlessly between portrait navigation and landscape gameplay.

---

## Architecture Flow

```text
               +----------------------+
               |    BootstrapFlow     | ---> Loading, Brand Logos & Intro Video
               +----------+-----------+
                          |
                          v
               +----------------------+
               |      IntroMenu       | ---> IntroScreen & MainMenuIntroController
               +----------+-----------+
                          |
                          v
               +----------------------+
               |    Questionnaire     | <---> GameEvents (ScriptableObject Event Bus)
               +----------+-----------+        |-- QuestionnaireManager (Data & Scoring)
                          |                    \-- UIManager (Dynamic Button Instantiation)
                          v
               +----------------------+
               | QuestionnaireRouter  | ---> ActiveCareerController (Shared Runtime State)
               +----------+-----------+        \-- GameManager (Persistent Feature Flags)
                          |
         +----------------+----------------+
         |                                 |
         v                                 v
+------------------+             +------------------+
|  Direct Testing  |             |  Explorer World  |
|  (Non-Explorer)  |             | (Explorer Type)  |
+--------+---------+             +--------+---------+
         |                                |
         +----------------+---------------+
                          |
                          v
               +----------------------+
               |   3 Work-Trials      | ---> CareerSO (Data-Driven Questions & Audio)
               | (Dynamic Orientation)| ---> SceneOrientationController (Portrait/Landscape)
               +----------------------+
