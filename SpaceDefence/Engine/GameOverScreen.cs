using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
  public class GameOverScreen
  {
    private bool _active = false;
    private float _elapsed = 0f;
    public bool IsActive => _active;

    private const float BgFadeInDuration = 1.2f;
    private const float TextAppearTime = 1.5f;
    private const float TextFadeDuration = 0.8f;
    private const float ButtonAppearTime = 3.0f;
    private const float ButtonFadeDuration = 0.5f;

    private readonly GraphicsDevice _gd;
    private readonly SpriteBatch _sb;
    private SpriteFont _fontLarge;
    private SpriteFont _fontSmall;
    private readonly Texture2D _pixel;

    private Rectangle _buttonRect;
    private bool _buttonHovered = false;
    private MouseState _prevMouse;

    private readonly Action _onRespawn;

    private static readonly Color ColBgDark = new Color(80, 5, 5);
    private static readonly Color ColBgStripe = new Color(160, 20, 20);
    private static readonly Color ColGold = new Color(218, 165, 32);
    private static readonly Color ColBorder = new Color(180, 80, 20);
    private static readonly Color ColBtnIdle = new Color(40, 5, 5);
    private static readonly Color ColBtnHover = new Color(140, 15, 15);

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
      _buttonRect = new Rectangle(cx - bw / 2, cy + 90, bw, bh);
    }

    public void Activate() { _active = true; _elapsed = 0f; }
    public void Deactivate() { _active = false; _elapsed = 0f; }

    public void Update(GameTime gameTime)
    {
      if (!_active) return;
      _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

      MouseState mouse = Mouse.GetState();
      if (_elapsed >= ButtonAppearTime)
      {
        _buttonHovered = _buttonRect.Contains(mouse.Position);
        if (_buttonHovered
            && mouse.LeftButton == ButtonState.Released
            && _prevMouse.LeftButton == ButtonState.Pressed)
        {
          Alien.ResetSpeed();
          Deactivate();
          _onRespawn?.Invoke();
        }
      }
      _prevMouse = mouse;
    }

    public void Draw(GameTime gameTime)
    {
      if (!_active) return;
      int W = _gd.Viewport.Width, H = _gd.Viewport.Height;
      int cx = W / 2, cy = H / 2;

      _sb.Begin();

      // 1. Dark-red overlay fading in
      float bgA = MathHelper.Clamp(_elapsed / BgFadeInDuration, 0f, 1f);
      DrawRect(new Rectangle(0, 0, W, H), ColBgDark * (bgA * 0.85f));
      DrawRect(new Rectangle(0, cy - 80, W, 160), ColBgStripe * (bgA * 0.18f));

      // 2. "YOU DIED"
      if (_elapsed >= TextAppearTime && _fontLarge != null)
      {
        float t = MathHelper.Clamp((_elapsed - TextAppearTime) / TextFadeDuration, 0f, 1f);
        DrawTitle(cx, cy - 30, t);
      }

      // 3. Respawn button
      if (_elapsed >= ButtonAppearTime && _fontSmall != null)
      {
        float t = MathHelper.Clamp((_elapsed - ButtonAppearTime) / ButtonFadeDuration, 0f, 1f);
        DrawButton(t);
      }

      _sb.End();
    }

    private void DrawTitle(int cx, int cy, float alpha)
    {
      string text = "YOU DIED";
      Vector2 size = _fontLarge.MeasureString(text);
      Vector2 pos = new Vector2(cx - size.X / 2f, cy - size.Y / 2f);

      // Shadow
      for (int ox = -4; ox <= 4; ox += 2)
        for (int oy = -4; oy <= 4; oy += 2)
          _sb.DrawString(_fontLarge, text, pos + new Vector2(ox, oy), Color.Black * (alpha * 0.55f));

      // Gold text
      _sb.DrawString(_fontLarge, text, pos, ColGold * alpha);

      // Rule beneath
      int rW = (int)(size.X * 1.15f), rX = cx - rW / 2, rY = (int)(pos.Y + size.Y + 4);
      DrawRect(new Rectangle(rX, rY, rW, 2), ColBorder * alpha);
      DrawRect(new Rectangle(rX + 4, rY + 4, rW - 8, 1), ColBgStripe * (alpha * 0.5f));
    }

    private void DrawButton(float alpha)
    {
      DrawRect(_buttonRect, (_buttonHovered ? ColBtnHover : ColBtnIdle) * alpha);
      DrawBorder(_buttonRect, ColBorder * alpha, 2);

      string label = "RESPAWN";
      Vector2 lSz = _fontSmall.MeasureString(label);
      Vector2 lPos = new Vector2(
          _buttonRect.X + (_buttonRect.Width - lSz.X) / 2f,
          _buttonRect.Y + (_buttonRect.Height - lSz.Y) / 2f);

      _sb.DrawString(_fontSmall, label, lPos,
                     (_buttonHovered ? new Color(255, 230, 120) : ColGold) * alpha);
    }

    private void DrawRect(Rectangle r, Color c) => _sb.Draw(_pixel, r, c);

    private void DrawBorder(Rectangle r, Color c, int t)
    {
      DrawRect(new Rectangle(r.X, r.Y, r.Width, t), c);
      DrawRect(new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
      DrawRect(new Rectangle(r.X, r.Y, t, r.Height), c);
      DrawRect(new Rectangle(r.Right - t, r.Y, t, r.Height), c);
    }
  }
}
