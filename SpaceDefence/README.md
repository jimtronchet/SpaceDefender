# Space Defence

A 2D space shooter built with MonoGame, as assignment for explorative Game Development.

## How to Play

- **WASD** accelerate the ship (momentum-based, can't brake in space!)
- **Mouse** aim the turret
- **Left click** shoot
- **Escape** pause / unpause

Survive as long as possible. Every alien you kill spawns a faster one. Don't let anything touch you. As tiems goes on. More aliens and asteroids spawn to destroy you!

## Core features

- **Movement** WASD accelerates the ship, velocity is conserved
- **Ship rotation** ship faces the last direction it accelerated
- **Alien Behaviour** aliens move toward the player to kill them, this also ends the game. each alien killed spawns a replacement that moves faster.
- **Collision** Fully implemented collisions. (Some small bugs with asteroids I had no time to fix.)
- **Start screen** title screen with Start and Quit buttons
- **Pause screen** Escape pauses the game
- **Camera** follows the player
- **Scrolling background** - uses assignment assets
- **Asteroid Behaviour** s destroys player and aliens on contact
- **Difficulty** `SpawnManager` spawns additional aliens every 10s (interval shrinks to 3s minimum) and asteroids every 15s (down to 6s minimum)

## Extra Features (beyond the assignment)

### Game Over Screen

A overlay like dark souls appears when the player dies. Clicking respawn resets the game.
Custom fonts were used and implemented in these UI screen.

### Background Parallax Layers

Two tiled textures cover the world space

### Laser following turret direction

The laser is fired from the tip of the turret and travels in the direction the turret is currently pointing. It always follows the turret's rotation as the player aims with the mouse.
