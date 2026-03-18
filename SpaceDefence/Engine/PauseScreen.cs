using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
  public class PauseScreen
  {
    public bool IsActive { get; private set; } = false;

    // Delay before the pause screen accepts any input, so the same Escape
    // press that opened it doesn't immediately close it again.
    private float _inputDelay = 0f;
    private const float InputDelayTime = 0.2f;

    private readonly GraphicsDevice _gd;
    private readonly SpriteBatch _sb;
    private SpriteFont _font;
    private readonly Texture2D _pixel;

    private Rectangle _btnContinue, _btnQuit;
    private bool _hoverContinue, _hoverQuit;
    private MouseState _prevMouse;

    private readonly Action _onContinue;
    private readonly Action _onQuit;

    public PauseScreen(GraphicsDevice gd, Action onContinue, Action onQuit)
    {
      _gd = gd;
      _onContinue = onContinue;
      _onQuit = onQuit;
      _sb = new SpriteBatch(gd);
      _pixel = new Texture2D(gd, 1, 1);
      _pixel.SetData(new[] { Color.White });
    }

    public void SetFonts(SpriteFont font)
    {
      _font = font;

      int cx = _gd.Viewport.Width / 2;
      int cy = _gd.Viewport.Height / 2;
      int bw = 220, bh = 44;

      _btnContinue = new Rectangle(cx - bw / 2, cy + 20, bw, bh);
      _btnQuit = new Rectangle(cx - bw / 2, cy + 20 + 64, bw, bh);
    }

    public void Activate()
    {
      IsActive = true;
      _inputDelay = 0f;   // reset delay every time we open
    }

    public void Deactivate() => IsActive = false;

    public void Toggle()
    {
      if (IsActive) Deactivate();
      else Activate();
    }

    public void Update(GameTime gameTime)
    {
      if (!IsActive) return;

      // Count up the input delay — buttons and Escape are ignored until it expires
      _inputDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (_inputDelay < InputDelayTime) return;

      MouseState mouse = Mouse.GetState();
      KeyboardState keys = Keyboard.GetState();

      _hoverContinue = _btnContinue.Contains(mouse.Position);
      _hoverQuit = _btnQuit.Contains(mouse.Position);

      // Escape closes pause
      if (keys.IsKeyDown(Keys.Escape))
      {
        Deactivate();
        _onContinue?.Invoke();
        return;
      }

      bool released = mouse.LeftButton == ButtonState.Released
                   && _prevMouse.LeftButton == ButtonState.Pressed;
      if (released)
      {
        if (_hoverContinue) { Deactivate(); _onContinue?.Invoke(); }
        if (_hoverQuit) { _onQuit?.Invoke(); }
      }

      _prevMouse = mouse;
    }

    public void Draw(GameTime gameTime)
    {
      if (!IsActive || _font == null) return;

      int W = _gd.Viewport.Width, H = _gd.Viewport.Height;
      int cx = W / 2, cy = H / 2;

      _sb.Begin();

      // Semi-transparent black overlay — game world stays visible behind it
      _sb.Draw(_pixel, new Rectangle(0, 0, W, H), Color.Black * 0.65f);

      // Title
      DrawTextCentered("PAUSED", cx, cy - 100, Color.White);

      // Buttons
      DrawButton(_btnContinue, "CONTINUE", _hoverContinue);
      DrawButton(_btnQuit, "QUIT", _hoverQuit);

      _sb.End();
    }

    private void DrawButton(Rectangle rect, string label, bool hovered)
    {
      Color fill = hovered ? Color.White : Color.Black;
      Color textColor = hovered ? Color.Black : Color.White;

      _sb.Draw(_pixel, rect, fill);
      DrawBorder(rect, Color.White, 2);

      Vector2 sz = _font.MeasureString(label);
      Vector2 pos = new Vector2(
          rect.X + (rect.Width - sz.X) / 2f,
          rect.Y + (rect.Height - sz.Y) / 2f);
      _sb.DrawString(_font, label, pos, textColor);
    }

    private void DrawTextCentered(string text, int cx, int cy, Color color)
    {
      Vector2 sz = _font.MeasureString(text);
      Vector2 pos = new Vector2(cx - sz.X / 2f, cy - sz.Y / 2f);
      _sb.DrawString(_font, text, pos, color);
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
