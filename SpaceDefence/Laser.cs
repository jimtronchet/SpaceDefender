using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Laser : GameObject
    {
        private LinePieceCollider linePiece;
        private Texture2D sprite;
        private double lifespan = .25f;
        private Ship parentTurret;
        private Vector2 localStart;
        private Vector2 localDirection;
        private bool useLocalSpace;

        public Laser(LinePieceCollider linePiece)
        {
            this.linePiece = linePiece;
            SetCollider(linePiece);
        }
        public Laser(LinePieceCollider linePiece, float length) : this(linePiece)
        {
            // Sets the length of the laser to be equal to the width of the screen, so it will always cover the full screen.
            this.linePiece.Length = length;
        }

        public Laser(Ship parentTurret, Vector2 localStart, Vector2 localDirection, float length)
        {
            this.parentTurret = parentTurret;
            this.localStart = localStart;
            this.localDirection = localDirection;
            if (this.localDirection == Vector2.Zero)
                this.localDirection = -Vector2.UnitY;
            this.localDirection.Normalize();
            useLocalSpace = true;

            linePiece = new LinePieceCollider(Vector2.Zero, Vector2.Zero);
            SetCollider(linePiece);
            linePiece.Length = length;
            UpdateWorldFromLocal();
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            sprite = content.Load<Texture2D>("Laser");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (useLocalSpace)
                UpdateWorldFromLocal();

            if (lifespan < 0)
                GameManager.GetGameManager().RemoveGameObject(this);
            lifespan -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void UpdateWorldFromLocal()
        {
            if (parentTurret == null)
                return;

            float rotation = parentTurret.GetTurretWorldRotation();
            float sin = (float)System.Math.Sin(rotation);
            float cos = (float)System.Math.Cos(rotation);

            Vector2 rotatedLocalStart = new Vector2(
                localStart.X * cos - localStart.Y * sin,
                localStart.X * sin + localStart.Y * cos);


            //  Rotate the local direction vector by the turret's rotation to get the world direction
            Vector2 rotatedLocalDirection = new Vector2(
                localDirection.X * cos - localDirection.Y * sin,
                localDirection.X * sin + localDirection.Y * cos);

            if (rotatedLocalDirection != Vector2.Zero)
                rotatedLocalDirection.Normalize();

            // Set the line piece's start and end points based on the rotated local start and direction, and the turret's world position
            float length = linePiece.Length;
            linePiece.Start = parentTurret.GetTurretWorldPosition() + rotatedLocalStart;
            linePiece.End = linePiece.Start + rotatedLocalDirection * length;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle target = new Rectangle((int)linePiece.Start.X, (int)linePiece.Start.Y, sprite.Width, (int)linePiece.Length);
            spriteBatch.Draw(sprite, target, null, Color.White, linePiece.GetAngle(), new Vector2(sprite.Width / 2f, sprite.Height), SpriteEffects.None, 1);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
