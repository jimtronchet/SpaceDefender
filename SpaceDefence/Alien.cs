using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Alien : GameObject
    {
        private CircleCollider _circleCollider;
        private Texture2D _texture;

        private const float KillRange = 60f; // Radius at which alien kills the player;

        private const float PlayerClearance = 100f; // Make sure aliens don't spawn too close to the player

        private static float _baseSpeed = 80f; // Increased each time an alien is killeds
        private const float SpeedIncrement = 20f;

        private float _chaseSpeed; // This alien's speeds

        public Alien()
        {
            _chaseSpeed = _baseSpeed;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("Alien");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);
            RandomMove();
        }

        public override void Update(GameTime gameTime)
        {
            GameManager gm = GameManager.GetGameManager();
            Vector2 playerCenter = gm.Player.GetPosition().Center.ToVector2();
            Vector2 toPlayer = playerCenter - _circleCollider.Center;
            float distance = toPlayer.Length();

            if (distance <= KillRange)
            {
                gm.GameOver();
                return;
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _circleCollider.Center += (toPlayer / distance) * _chaseSpeed * delta;

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other)
        {
            // Ignore the player — only bullets/other objects kill an alien
            if (other is Ship)
                return;

            // Speed up the next alien and spawn it before removing self
            _baseSpeed += SpeedIncrement;
            GameManager gm = GameManager.GetGameManager();
            gm.AddGameObject(new Alien());
            gm.RemoveGameObject(this);

            base.OnCollision(other);
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            Vector2 playerCenter = gm.Player.GetPosition().Center.ToVector2();

            do
            {
                _circleCollider.Center = gm.RandomScreenLocation();
            }
            while ((_circleCollider.Center - playerCenter).Length() < PlayerClearance);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            base.Draw(gameTime, spriteBatch);
        }

        public static void ResetSpeed()
        {
            _baseSpeed = 80f;
        }
    }
}
