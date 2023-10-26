# CS328-Semester-Project

Unity Version: 2022.3.9f1 (LTS)

### Vesion 0.2.1
- Added dash mechanic to player controller
    - Not entirely happy with the implementation. There is a better way to do this, will revist later.

### Version 0.2.0 
- Updated Player prefab so that movement speed is now on a different scale
- Adjusted player speed and bullet speed to that the player does not outrun the bullets
- Fixed an issue where the Level 2 button would load level 1

### Version 0.1.5
- Added new temporary geometry to levels 1 & 2.

### Version 0.1.4
- Player now looks in mouse direction
- Added bullet prefab and sprite
- Player now can shoot

### Version 0.1.3
- Reformatted layout of scripts to script file
- Added an animation folder
- Added a sprite folder
- Added UI overlay
    - Displays current health
    - Displays current mana
- Added Pause Menu functionality
    - Can pause and unpause the game
        - Stops the game in background
    - Can exit to the main menu
    - Can exit the game

### Version 0.1.2
- Redid the player movement script to utilize RigidBody2D.movePosition() instead of transform.position
    - This should avoid a jittering issue when colliding with walls
- Added a camera controller script to the main camera
    - It just simply follows the player around
- Created a wall (with collision) to build levels with
- Made prefabs out of these GameObjects to allow for easy level creation
- Added these prefabs to the two exisitng levels with some basic geometry to differentiate them

### Version 0.1.1
- Added Main Menu scene, functionality, and UI
    - Allows switching between scenes and exiting the game
- Created level select scene
    - Allows switching between levels and going back to the main menu
- Updated player movement to be able to use WASD keys as well as arrow keys