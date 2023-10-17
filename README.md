# CS328-Semester-Project

Unity Version: 2022.3.9f1 (LTS)

### Version 0.1.2
- Redid the player movement script to utilize RigidBody2D.movePosition() instead of transform.position
    - This should avoid a jittering issue when colliding with walls
- Added a camera controller script to the main camera
    - It just simply follows the player around
- Created a wall (with collision) to build levels with
- Made prefabs out of these GameObjects to allow for easy level creation
- Added these prefabs to the two exisitng levels with some basic geometry to differentiate them