using Microsoft.Xna.Framework;

namespace SpaceDefence
{
  /// <summary>
  /// Handles difficulty ramping by spawning extra aliens and asteroids over time.
  /// Add one instance to the GameManager and call Update each frame.
  /// </summary>
  public class SpawnManager
  {
    // ── Alien spawning ────────────────────────────────────────────────────
    // Start by spawning a new alien every 10 seconds.
    // Each spawn the interval shrinks a little, down to a minimum of 3 s.
    private float _alienTimer = 0f;
    private float _alienInterval = 10f;
    private const float AlienIntervalMin = 3f;
    private const float AlienIntervalDecrease = 0.5f;  // shaved off after each spawn

    // ── Asteroid spawning ─────────────────────────────────────────────────
    // Spawn a new asteroid every 15 seconds, respecting the hard cap.
    private float _asteroidTimer = 0f;
    private float _asteroidInterval = 15f;
    private const float AsteroidIntervalMin = 6f;
    private const float AsteroidIntervalDecrease = 1f;

    public void Reset()
    {
      _alienTimer = 0f;
      _alienInterval = 10f;
      _asteroidTimer = 0f;
      _asteroidInterval = 15f;
    }

    public void Update(GameTime gameTime)
    {
      float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
      GameManager gm = GameManager.GetGameManager();

      // ── Alien ramp-up ─────────────────────────────────────────────────
      _alienTimer += dt;
      if (_alienTimer >= _alienInterval)
      {
        _alienTimer = 0f;
        gm.AddGameObject(new Alien());

        // Shrink the interval so spawns come faster over time
        _alienInterval = MathHelper.Max(
            _alienInterval - AlienIntervalDecrease,
            AlienIntervalMin);
      }

      // ── Asteroid ramp-up ──────────────────────────────────────────────
      _asteroidTimer += dt;
      if (_asteroidTimer >= _asteroidInterval)
      {
        _asteroidTimer = 0f;

        if (Asteroid.CanSpawn())
          gm.AddGameObject(new Asteroid());

        _asteroidInterval = MathHelper.Max(
            _asteroidInterval - AsteroidIntervalDecrease,
            AsteroidIntervalMin);
      }
    }
  }
}
