using System;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class Ship : GameObject
    {
        private Texture2D ship_body;
        private Texture2D base_turret;
        private Texture2D laser_turret;
        private float buffTimer = 0f;
        private float buffDuration = 10f;
        private RectangleCollider _rectangleCollider;
        private Point target;
        private float acceleration = 250f;
        private float shipRotation = 0f;
        private Vector2 velocity = Vector2.Zero;
        private Vector2 positionFloat = Vector2.Zero;
        private float turretRotation = 0f;

        /// <summary>
        /// The player character
        /// </summary>
        /// <param name="Position">The ship's starting position</param>
        public Ship(Point Position)
        {
            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
            SetCollider(_rectangleCollider);
        }

        public override void Load(ContentManager content)
        {
            // Ship sprites from: https://zintoki.itch.io/space-breaker
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");
            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);
            positionFloat = _rectangleCollider.shape.Center.ToVector2();
            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);

            float delta = (float)GameManager.GetGameManager().Game.TargetElapsedTime.TotalSeconds;

            // Build raw input direction from WASD
            Vector2 inputDirection = Vector2.Zero;
            if (inputManager.IsKeyDown(Keys.W))
                inputDirection.Y -= 1;
            if (inputManager.IsKeyDown(Keys.S))
                inputDirection.Y += 1;
            if (inputManager.IsKeyDown(Keys.A))
                inputDirection.X -= 1;
            if (inputManager.IsKeyDown(Keys.D))
                inputDirection.X += 1;


            // We are moving
            if (inputDirection != Vector2.Zero)
            {
                inputDirection.Normalize();

                // Accelerate: add to velocity each frame proportional to delta time
                velocity += inputDirection * acceleration * delta;

                // Rotate ship to face the direction of the last acceleration input
                shipRotation = LinePieceCollider.GetAngle(inputDirection);
            }

            // Apply velocity to position
            positionFloat += velocity * delta;

            // Clamp position to viewport and zero out velocity component on collision with border
            var viewport = GameManager.GetGameManager().Game.GraphicsDevice.Viewport;
            Vector2 halfSize = _rectangleCollider.shape.Size.ToVector2() / 2f;

            if (positionFloat.X < halfSize.X)
            {
                positionFloat.X = halfSize.X;
                if (velocity.X < 0) velocity.X = 0;
            }
            else if (positionFloat.X > viewport.Width - halfSize.X)
            {
                positionFloat.X = viewport.Width - halfSize.X;
                if (velocity.X > 0) velocity.X = 0;
            }

            if (positionFloat.Y < halfSize.Y)
            {
                positionFloat.Y = halfSize.Y;
                if (velocity.Y < 0) velocity.Y = 0;
            }
            else if (positionFloat.Y > viewport.Height - halfSize.Y)
            {
                positionFloat.Y = viewport.Height - halfSize.Y;
                if (velocity.Y > 0) velocity.Y = 0;
            }

            // Sync collider to the float position
            _rectangleCollider.shape.Location = (positionFloat - halfSize).ToPoint();

            // Turret always aims at mouse cursor
            target = inputManager.CurrentMouseState.Position;
            Vector2 shipCenter = _rectangleCollider.shape.Center.ToVector2();
            Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center, target);
            turretRotation = LinePieceCollider.GetAngle(aimDirection);

            if (inputManager.LeftMousePress())
            {
                Vector2 turretOrigin = base_turret.Bounds.Size.ToVector2() / 2f;
                Vector2 turretExit = shipCenter + aimDirection * (turretOrigin.Y);

                if (buffTimer <= 0)
                {
                    GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
                }
                else
                {
                    Vector2 localMuzzleOffset = new Vector2(0f, -turretOrigin.Y);
                    GameManager.GetGameManager().AddGameObject(new Laser(this, localMuzzleOffset, -Vector2.UnitY, 400f));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update the Buff timer
            if (buffTimer > 0)
                buffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 shipCenter = _rectangleCollider.shape.Center.ToVector2();
            Vector2 shipOrigin = ship_body.Bounds.Size.ToVector2() / 2f;
            spriteBatch.Draw(ship_body, shipCenter, null, Color.White, shipRotation, shipOrigin, 1f, SpriteEffects.None, 0);

            Vector2 turretOrigin = base_turret.Bounds.Size.ToVector2() / 2f;
            if (buffTimer <= 0)
            {
                spriteBatch.Draw(base_turret, shipCenter, null, Color.White, turretRotation, turretOrigin, 1f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(laser_turret, shipCenter, null, Color.White, turretRotation, turretOrigin, 1f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime, spriteBatch);
        }

        public void Buff()
        {
            buffTimer = buffDuration;
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }

        public Vector2 GetTurretWorldPosition()
        {
            return _rectangleCollider.shape.Center.ToVector2();
        }

        public float GetTurretWorldRotation()
        {
            return turretRotation;
        }
    }
}
