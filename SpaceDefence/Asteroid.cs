using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
  internal class Asteroid : GameObject
  {
    private CircleCollider _circleCollider;
    private Texture2D _texture;

    private const int MaxAsteroids = 8;    // cap on how many can exist at once
    private const int BulletHitsToDestroy = 3;  // bullets needed to destroy
    private int _health;

    // Tracks how many asteroids currently exist so we never exceed the cap
    public static int ActiveCount = 0;

    public Asteroid()
    {
      _health = BulletHitsToDestroy;
      ActiveCount++;
    }

    private const float Scale = 2f;

    public override void Load(ContentManager content)
    {
      base.Load(content);
      _texture = content.Load<Texture2D>("asteroid");
      // Collider radius is half the texture width times the scale factor
      _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2 * Scale);
      SetCollider(_circleCollider);
      RandomPlace();
    }

    public override void OnCollision(GameObject other)
    {
      GameManager gm = GameManager.GetGameManager();

      if (other is Laser)
      {
        // One laser hit destroys the asteroid immediately
        Destroy();
        gm.RemoveGameObject(this);
        return;
      }

      if (other is Bullet)
      {
        _health--;
        if (_health <= 0)
        {
          Destroy();
          gm.RemoveGameObject(this);
        }
        return;
      }

      if (other is Ship)
      {
        // Touching the player triggers game over
        gm.GameOver();
        return;
      }

      if (other is Alien)
      {
        // Destroys the alien that walks into it; spawn a replacement
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
      // Draw at Scale size, centred on the collider centre
      Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
      spriteBatch.Draw(_texture, _circleCollider.Center, null, Color.White,
                       0f, origin, Scale, SpriteEffects.None, 0);
      base.Draw(gameTime, spriteBatch);
    }

    public static bool CanSpawn() => ActiveCount < MaxAsteroids;
  }
}
