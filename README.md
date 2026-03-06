# Water Sort Puzzle Prototype

A gameplay prototype of "Magic Bloom: Sort Water" created for the Sonat Intern Test. Developed using Unity 6.

🎥 **[Demo YouTube](https://youtu.be/2DeWXduCSqg)**
## Features
* **Core Mechanics:** Basic water pouring and color sorting logic.
* **Level Design:** 4 playable levels configured using ScriptableObjects (`LevelData`). Each level contains exactly 8 tubes.
* **Game States:** Implemented Win and Lose (deadlock/out of moves detection) conditions.
* **Architecture:** Uses the Singleton pattern for `GameManager` and `AudioManager`.
* **Animations:** Tube movement and liquid scaling implemented with DOTween.
* **UI & Audio:** Custom 2D UI with a simple frame-swap script for visual feedback, alongside independent BGM and SFX toggles.

## How to Run
1. Clone this repository.
2. Open the project in **Unity 6** (or Unity 2022.3).
3. Open the main scene located at `Assets/Scenes/Gameplay.unity`.
4. Press Play.