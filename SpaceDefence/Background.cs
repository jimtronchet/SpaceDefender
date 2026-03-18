using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
  /// <summary>
  /// Draws the two-layer scrolling background:
  ///   Layer 1 = void.png   tiled across the whole world, opaque base.
  ///   Layer 2  = stars.png  tiled on top, semi-transparent overlay.
  /// Both textures tile seamlessly to fill the world.
  /// </summary>
  public class Background
  {
    private Texture2D _void;
    private Texture2D _stars;

    public void Load(ContentManager content)
    {
      _void = content.Load<Texture2D>("void");
      _stars = content.Load<Texture2D>("stars");
    }

    /// <summary>
    /// Draws the background. Call this before drawing game objects.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Camera camera)
    {
      if (_void == null || _stars == null) return;

      Viewport viewport = GameManager.GetGameManager().Game.GraphicsDevice.Viewport;

      // Figure out which tiles are visible by snapping camera position to tile grid
      int startX = (int)(camera.Position.X / _void.Width) * _void.Width;
      int startY = (int)(camera.Position.Y / _void.Height) * _void.Height;
      int endX = (int)camera.Position.X + viewport.Width + _void.Width;
      int endY = (int)camera.Position.Y + viewport.Height + _void.Height;

      // Draw the base layer
      for (int x = startX; x < endX; x += _void.Width)
        for (int y = startY; y < endY; y += _void.Height)
          spriteBatch.Draw(_void, new Vector2(x, y), Color.White);

      // Draw the stars on top, slightly transparent
      for (int x = startX; x < endX; x += _stars.Width)
        for (int y = startY; y < endY; y += _stars.Height)
          spriteBatch.Draw(_stars, new Vector2(x, y), Color.White * 0.6f);
    }
  }
}
