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
        private float speed = 250f;
        private float shipRotation = 0f;
        private Vector2 lastMovementDirection = Vector2.Zero;
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

            // WASD movement
            Vector2 movement = Vector2.Zero;
            if (inputManager.IsKeyDown(Keys.W))
                movement.Y -= 1;
            if (inputManager.IsKeyDown(Keys.S))
                movement.Y += 1;
            if (inputManager.IsKeyDown(Keys.A))
                movement.X -= 1;
            if (inputManager.IsKeyDown(Keys.D))
                movement.X += 1;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                lastMovementDirection = movement;
                shipRotation = LinePieceCollider.GetAngle(movement);

                // Use a float position accumulator to avoid integer truncation causing slower diagonal movement
                float delta = (float)GameManager.GetGameManager().Game.TargetElapsedTime.TotalSeconds;
                Vector2 displacement = movement * speed * delta;
                positionFloat += displacement;

                // Clamp position using ship center and half-size
                var viewport = GameManager.GetGameManager().Game.GraphicsDevice.Viewport;
                Vector2 halfSize = _rectangleCollider.shape.Size.ToVector2() / 2f;
                positionFloat.X = Math.Max(halfSize.X, Math.Min(positionFloat.X, viewport.Width - halfSize.X));
                positionFloat.Y = Math.Max(halfSize.Y, Math.Min(positionFloat.Y, viewport.Height - halfSize.Y));

                // Sync collider location to the rounded position
                _rectangleCollider.shape.Location = (positionFloat - halfSize).ToPoint();
            }

            target = inputManager.CurrentMouseState.Position;
            Vector2 shipCenter = _rectangleCollider.shape.Center.ToVector2();
            Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center, target);
            turretRotation = LinePieceCollider.GetAngle(aimDirection);

            if (inputManager.LeftMousePress())
            {
                Vector2 turretOrigin = base_turret.Bounds.Size.ToVector2() / 2f;

                // turret tip in world space: from ship center move along aim direction by half the turret height
                Vector2 turretExit = shipCenter + aimDirection * (turretOrigin.Y);

                if (buffTimer <= 0)
                {
                    GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
                }
                else
                {
                    // Fire in turret local space and resolve to world space via the ship turret transform.
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
                // Draw the turret centered on the ship so rotation keeps it attached
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
