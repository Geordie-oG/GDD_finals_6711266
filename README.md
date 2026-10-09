# Three in One Game

CSX4515 Game Design and Development, Take-Home Final 1/2026, by Ye Htet Aung (6711266).

One Unity 6 project that combines three games built in class, behind a shared Main Menu and In-Game Menu.

| Main Menu button | Scene | Reused class game |
| --- | --- | --- |
| Mad Driver | `Assets/Scenes/DrivingGame.unity` | Prototype 1 (driving) |
| Fly Like a Bird | `Assets/Scenes/FlyingGame.unity` | Challenge 1 (plane) |
| I'm a Sumo and a Ball | `Assets/Scenes/SumoGame.unity` | Prototype 4 (sumo ball) |
| Exit | n/a | Quits the game (stops Play Mode in the Editor) |

## Run

Open the project in Unity 6000.5.2f1, open `Assets/Scenes/MainMenu.unity`, and press Play.

## Controls

- **Mad Driver:** W/S or Up/Down to drive, A/D or Left/Right to steer.
- **Fly Like a Bird:** the plane flies forward by itself. W/S or Up/Down tilt it up and down.
- **I'm a Sumo and a Ball:** W/S or Up/Down roll the ball, A/D or Left/Right rotate the camera. Push the enemies off the island and grab the powerup.
- **Every game:** Escape opens the In-Game Menu (PAUSED). Choose Resume, Restart, or Back to Main Menu. Escape again also resumes.

## Code

- `Assets/Scripts/MainMenu.cs`: Main Menu buttons and Exit.
- `Assets/Scripts/PauseMenu.cs`: In-Game Menu (pause, resume, restart, back to menu).
- `Assets/Driving/Scripts/`: Prototype 1 vehicle and camera scripts.
- `Assets/Challenge 1/Scripts/`: Challenge 1 plane, camera and propeller scripts.
- `Assets/Sumo/Scripts/`: Prototype 4 player, enemy, spawn manager and camera scripts.
- `Assets/Editor/ExamSceneBuilder.cs`: menu **Three In One > Build All Scenes** builds the menus into the scenes.
- `Assets/Editor/ExamValidator.cs`: menu **Three In One > Validate Exam Project** checks every requirement.
