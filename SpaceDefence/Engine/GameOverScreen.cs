using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
  public class GameOverScreen
  {
    public bool IsActive { get; private set; } = false;

    private readonly GraphicsDevice _gd;
    private readonly SpriteBatch _sb;
    private SpriteFont _fontLarge;
    private SpriteFont _fontSmall;
    private readonly Texture2D _pixel;

    private Rectangle _buttonRect;
    private bool _buttonHovered;
    private MouseState _prevMouse;

    private readonly Action _onRespawn;

    public GameOverScreen(GraphicsDevice graphicsDevice, Action onRespawn)
    {
      _gd = graphicsDevice;
      _onRespawn = onRespawn;
      _sb = new SpriteBatch(graphicsDevice);
      _pixel = new Texture2D(graphicsDevice, 1, 1);
      _pixel.SetData(new[] { Color.White });
    }

    public void SetFonts(SpriteFont fontLarge, SpriteFont fontSmall)
    {
      _fontLarge = fontLarge;
      _fontSmall = fontSmall;

      int bw = 260, bh = 54;
      int cx = _gd.Viewport.Width / 2;
      int cy = _gd.Viewport.Height / 2;
      _buttonRect = new Rectangle(cx - bw / 2, cy + 60, bw, bh);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void Update(GameTime gameTime)
    {
      if (!IsActive) return;

      MouseState mouse = Mouse.GetState();
      _buttonHovered = _buttonRect.Contains(mouse.Position);

      if (_buttonHovered
          && mouse.LeftButton == ButtonState.Released
          && _prevMouse.LeftButton == ButtonState.Pressed)
      {
        Alien.ResetSpeed();
        Deactivate();
        _onRespawn?.Invoke();
      }

      _prevMouse = mouse;
    }

    public void Draw(GameTime gameTime)
    {
      if (!IsActive || _fontLarge == null || _fontSmall == null) return;

      int W = _gd.Viewport.Width;
      int H = _gd.Viewport.Height;
      int cx = W / 2;
      int cy = H / 2;

      _sb.Begin();

      // Dark overlay
      _sb.Draw(_pixel, new Rectangle(0, 0, W, H), Color.Black * 0.75f);

      // "YOU DIED" centred on screen
      string title = "YOU DIED";
      Vector2 titleSize = _fontLarge.MeasureString(title);
      Vector2 titlePos = new Vector2(cx - titleSize.X / 2f, cy - titleSize.Y / 2f - 30);
      _sb.DrawString(_fontLarge, title, titlePos, new Color(180, 0, 0));

      // Respawn button
      Color fill = _buttonHovered ? Color.White : Color.Black;
      Color text = _buttonHovered ? Color.Black : Color.White;
      _sb.Draw(_pixel, _buttonRect, fill);
      DrawBorder(_buttonRect, Color.White, 2);

      string label = "RESPAWN";
      Vector2 labelSz = _fontSmall.MeasureString(label);
      Vector2 labelPos = new Vector2(
          _buttonRect.X + (_buttonRect.Width - labelSz.X) / 2f,
          _buttonRect.Y + (_buttonRect.Height - labelSz.Y) / 2f);
      _sb.DrawString(_fontSmall, label, labelPos, text);

      _sb.End();
    }

    private void DrawBorder(Rectangle r, Color c, int t)
    {
      _sb.Draw(_pixel, new Rectangle(r.X, r.Y, r.Width, t), c);
      _sb.Draw(_pixel, new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
      _sb.Draw(_pixel, new Rectangle(r.X, r.Y, t, r.Height), c);
      _sb.Draw(_pixel, new Rectangle(r.Right - t, r.Y, t, r.Height), c);
    }
  }
}
