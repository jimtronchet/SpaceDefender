using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
  /// <summary>
  /// A 2D camera that follows the player around a world larger than the screen.
  ///
  /// How it works:
  /// The camera keeps a "transform matrix" that SpriteBatch uses when drawing.
  /// This matrix shifts everything so the player stays centred on screen.
  /// Objects outside the visible area are still drawn — SpriteBatch skips them
  /// automatically because they fall outside the viewport after the transform.
  ///
  /// World coordinates:  the actual position of game objects in the large level.
  /// Screen coordinates: where those objects appear on the player's monitor.
  /// </summary>
  public class Camera
  {
    // ── World bounds ──────────────────────────────────────────────────────
    /// <summary>Total size of the play area in world coordinates.</summary>
    public static readonly int WorldWidth = 3840;
    public static readonly int WorldHeight = 2160;

    // ── State ─────────────────────────────────────────────────────────────
    /// <summary>The top-left corner of what the camera currently sees, in world space.</summary>
    public Vector2 Position { get; private set; }

    private readonly Viewport _viewport;

    public Camera(Viewport viewport)
    {
      _viewport = viewport;
      Position = Vector2.Zero;
    }

    /// <summary>
    /// Call every frame to keep the camera centred on the player.
    /// The position is clamped so the camera never shows outside the world.
    /// </summary>
    public void Follow(Vector2 target)
    {
      // We want the target at the centre of the screen, so the camera's
      // top-left corner should be (target - half screen size).
      float x = target.X - _viewport.Width / 2f;
      float y = target.Y - _viewport.Height / 2f;

      // Clamp so we never scroll past the world edges
      x = MathHelper.Clamp(x, 0, WorldWidth - _viewport.Width);
      y = MathHelper.Clamp(y, 0, WorldHeight - _viewport.Height);

      Position = new Vector2(x, y);
    }

    /// <summary>
    /// The transform matrix to pass to SpriteBatch.Begin().
    /// It translates every drawn object by -Position, so the world scrolls
    /// in the opposite direction the camera moves.
    /// </summary>
    public Matrix GetTransform()
    {
      return Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
    }

    /// <summary>
    /// Converts a screen-space point (e.g. mouse position) into world-space.
    /// Useful for aiming — the mouse reports screen coords but the game uses world coords.
    /// </summary>
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
      return screenPosition + Position;
    }
  }
}
