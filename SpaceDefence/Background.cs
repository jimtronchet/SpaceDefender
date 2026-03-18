using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
  /// <summary>
  /// Draws the two-layer scrolling background:
  ///   Layer 1 — void.png   tiled across the whole world, opaque base.
  ///   Layer 2 — stars.png  tiled on top, semi-transparent overlay.
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
    /// Draw the tiled background. Call this BEFORE drawing game objects,
    /// and pass the same camera transform that the rest of the world uses.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Camera camera)
    {
      if (_void == null || _stars == null) return;

      // Only draw tiles that the camera can currently see (culling)
      int startX = (int)(camera.Position.X / _void.Width) * _void.Width;
      int startY = (int)(camera.Position.Y / _void.Height) * _void.Height;
      int endX = (int)camera.Position.X + GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width + _void.Width;
      int endY = (int)camera.Position.Y + GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height + _void.Height;

      // Clamp to world bounds
      startX = (int)MathHelper.Clamp(startX, 0, Camera.WorldWidth);
      startY = (int)MathHelper.Clamp(startY, 0, Camera.WorldHeight);
      endX = (int)MathHelper.Clamp(endX, 0, Camera.WorldWidth);
      endY = (int)MathHelper.Clamp(endY, 0, Camera.WorldHeight);

      // Tile the void base layer
      for (int x = startX; x < endX; x += _void.Width)
        for (int y = startY; y < endY; y += _void.Height)
          spriteBatch.Draw(_void, new Vector2(x, y), Color.White);

      // Tile the stars overlay (semi-transparent)
      for (int x = startX; x < endX; x += _stars.Width)
        for (int y = startY; y < endY; y += _stars.Height)
          spriteBatch.Draw(_stars, new Vector2(x, y), Color.White * 0.6f);
    }
  }
}
