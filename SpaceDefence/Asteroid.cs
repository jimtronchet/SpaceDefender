using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
  internal class Asteroid : GameObject
  {
    private CircleCollider _circleCollider;
    private Texture2D _texture;
    private const float Scale = 2f;
    private const int MaxAsteroids = 8;

    public static int ActiveCount = 0;

    public Asteroid()
    {
      ActiveCount++;
    }

    public override void Load(ContentManager content)
    {
      base.Load(content);
      _texture = content.Load<Texture2D>("asteroid");
      _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2 * Scale);
      SetCollider(_circleCollider);
      RandomPlace();
    }

    public override void OnCollision(GameObject other)
    {
      GameManager gm = GameManager.GetGameManager();

      if (other is Bullet || other is Laser)
      {
        gm.RemoveGameObject(this);
        return;
      }

      if (other is Ship)
      {
        gm.GameOver();
        return;
      }

      if (other is Alien)
      {
        // Alien walks into asteroid and dies; spawn a replacement
        gm.RemoveGameObject(other);
        gm.AddGameObject(new Alien());
      }
    }

    public override void Destroy()
    {
      ActiveCount--;
      base.Destroy();
    }

    private void RandomPlace()
    {
      GameManager gm = GameManager.GetGameManager();
      Vector2 playerCenter = gm.Player.GetPosition().Center.ToVector2();

      do
      {
        _circleCollider.Center = gm.RandomScreenLocation();
      }
      while ((_circleCollider.Center - playerCenter).Length() < 150f);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
      spriteBatch.Draw(_texture, _circleCollider.Center, null, Color.White,
                       0f, origin, Scale, SpriteEffects.None, 0);
      base.Draw(gameTime, spriteBatch);
    }

    public static bool CanSpawn() => ActiveCount < MaxAsteroids;
  }
}
