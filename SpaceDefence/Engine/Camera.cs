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

    public Matrix GetTransform()
    {
      return Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
      return screenPosition + Position;
    }
  }
}
