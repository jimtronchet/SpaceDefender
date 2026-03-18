using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
  public class Camera
  {
    //Total width and height of the play area in world coordinates.
    public static readonly int WorldWidth = 3840;
    public static readonly int WorldHeight = 2160;

    //The top-left corner position of what the camera is currently showing, measured in world space
    public Vector2 Position { get; private set; }

    private readonly Viewport _viewport;

    public Camera(Viewport viewport)
    {
      _viewport = viewport;
      Position = Vector2.Zero;
    }

    // Updates the camera position each frame to keep it centered on the target.
    // The position is clamped so the camera never shows outside the world boundaries.
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

    // Returns the transform matrix to pass to SpriteBatch.Begin().
    // It moves every drawn object by the negative camera position to create the scrolling effect.
    public Matrix GetTransform()
    {
      return Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
    }

    // Converts a point from screen space (like mouse position) into world space coordinates.
    // This is useful for aiming where the mouse is in screen coordinates but the game needs world coordinates.
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
      return screenPosition + Position;
    }
  }
}
