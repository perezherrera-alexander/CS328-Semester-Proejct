## TODO
- Refactor the code
    - Both the shooting and the dashing need to get the mouse position and the player position but they are both doing it themselves. This should be turned into a function that both can call.
- Redo the dash mechanic
    - The dash mechanic is currently done using Rigibody2D.MovePosition(). This can be better done using Rigidbody2D.AddForce() but I coudln't get it to work at the time. I will revisit this later.
- Properly make use of InputActions
    - Currently, button presses and mouse clicks are not properly utilizing the InputActions system. This should be fixed.