# Space Defence

A 2D space shooter built with MonoGame, as assignment for explorative Game Development.

## How to Play

- **WASD** — accelerate the ship (momentum-based, can't brake in space!)
- **Mouse** — aim the turret
- **Left click** — shoot
- **Escape** — pause / unpause

Survive as long as possible. Every alien you kill spawns a faster one. Don't let anything touch you. As tiems goes on. More aliens and asteroids spawn to destroy you!

---

## Features

- **Momentum movement** — WASD accelerates the ship, velocity is conserved
- **Ship rotation** — ship faces the last direction it accelerated
- **Alien chasing** — aliens move toward the player to kill them
- **Speed escalation** — each alien killed spawns a replacement that moves faster
- **Game over on contact** — alien reaching the player ends the game
- **Collision system** — Circle/Rectangle/LinePiece intersections fully implemented
- **Start screen** — title screen with Start and Quit buttons
- **Pause screen** — Escape pauses the game; world is visible but frozen;
- **Camera** — follows the player across a 3840×2160 world
- **Scrolling background** - using assignment assets
- **Asteroid enemy** — static obstacle; destroys player and aliens on contact
- **Difficulty ramp** — `SpawnManager` spawns additional aliens every 10s (interval shrinks to 3s minimum) and asteroids every 15s (down to 6s minimum)

---

## Extra Features (beyond the assignment)

### Game Over Screen

A Dark Souls-style **"YOU DIED"** overlay appears when the player dies. It fades in as animation, then shows a **RESPAWN** button that resets the game.

### Background Parallax Layers

Two tiled textures (`void.png` + `stars.png`) cover the full world with efficient viewport culling only the tiles currently visible are drawn each frame.
