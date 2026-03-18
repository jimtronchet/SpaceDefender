using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
  /// <summary>
  /// A 2D camera that follows the player around a world larger than the screen.
  ///
  /// How it works:
  /// The camera keeps a "transform matrix" that SpriteBatch uses when drawing.
  /// This matrix shifts everything so the player stays centered on screen.
  /// Objects outside the visible area are still drawn — SpriteBatch skips them
  /// automatically because they fall outside the viewport after the transform.
  ///
  /// World coordinates: the actual position of game objects in the large level.
  /// Screen coordinates: where those objects appear on the player's monitor.
  /// </summary>
  public class Camera
  {
    // World bounds
    /// <summary>Total width and height of the play area in world coordinates.</summary>
    public static readonly int WorldWidth = 3840;
    public static readonly int WorldHeight = 2160;

    // Camera state
    /// <summary>The top-left corner position of what the camera is currently showing, measured in world space.</summary>
    public Vector2 Position { get; private set; }

    private readonly Viewport _viewport;

    public Camera(Viewport viewport)
    {
      _viewport = viewport;
      Position = Vector2.Zero;
    }

    /// <summary>
    /// Updates the camera position each frame to keep it centered on the target.
    /// The position is clamped so the camera never shows outside the world boundaries.
    /// </summary>
    public void Follow(Vector2 target)
    {
      // Center the target on screen by moving camera's top-left to (target - half screen size)
      float x = target.X - _viewport.Width / 2f;
      float y = target.Y - _viewport.Height / 2f;

      // Clamp to world edges so camera doesn't go past the boundaries
      x = MathHelper.Clamp(x, 0, WorldWidth - _viewport.Width);
      y = MathHelper.Clamp(y, 0, WorldHeight - _viewport.Height);

      Position = new Vector2(x, y);
    }

    /// <summary>
    /// Returns the transform matrix to pass to SpriteBatch.Begin().
    /// It moves every drawn object by the negative camera position to create the scrolling effect.
    /// </summary>
    public Matrix GetTransform()
    {
      return Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
    }

    /// <summary>
    /// Converts a point from screen space (like mouse position) into world space coordinates.
    /// This is useful for aiming where the mouse is in screen coordinates but the game needs world coordinates.
    /// </summary>
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
      return screenPosition + Position;
    }
  }
}
