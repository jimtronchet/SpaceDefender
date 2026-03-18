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
        private Texture2D _ship_body;
        private Texture2D _base_turret;
        private Texture2D _laser_turret;
        private float _buffTimer = 0f;
        private float _buffDuration = 10f;
        private RectangleCollider _rectangleCollider;
        private float _acceleration = 250f;
        private float _shipRotation = 0f;
        private Vector2 _velocity = Vector2.Zero;
        private Vector2 _positionFloat = Vector2.Zero;
        private float _turretRotation = 0f;

        public Ship(Point Position)
        {
            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
            SetCollider(_rectangleCollider);
        }

        public override void Load(ContentManager content)
        {
            _ship_body = content.Load<Texture2D>("ship_body");
            _base_turret = content.Load<Texture2D>("base_turret");
            _laser_turret = content.Load<Texture2D>("laser_turret");
            _rectangleCollider.shape.Size = _ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(_ship_body.Width / 2, _ship_body.Height / 2);
            _positionFloat = _rectangleCollider.shape.Center.ToVector2();
            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);

            float delta = (float)GameManager.GetGameManager().Game.TargetElapsedTime.TotalSeconds;

            // ── Movement ──────────────────────────────────────────────────────
            Vector2 inputDirection = Vector2.Zero;
            if (inputManager.IsKeyDown(Keys.W)) inputDirection.Y -= 1;
            if (inputManager.IsKeyDown(Keys.S)) inputDirection.Y += 1;
            if (inputManager.IsKeyDown(Keys.A)) inputDirection.X -= 1;
            if (inputManager.IsKeyDown(Keys.D)) inputDirection.X += 1;

            if (inputDirection != Vector2.Zero)
            {
                inputDirection.Normalize();
                _velocity += inputDirection * _acceleration * delta;
                _shipRotation = LinePieceCollider.GetAngle(inputDirection);
            }

            _positionFloat += _velocity * delta;

            // Clamp to WORLD bounds (not viewport)
            Vector2 halfSize = _rectangleCollider.shape.Size.ToVector2() / 2f;

            if (_positionFloat.X < halfSize.X)
            { _positionFloat.X = halfSize.X; if (_velocity.X < 0) _velocity.X = 0; }
            else if (_positionFloat.X > Camera.WorldWidth - halfSize.X)
            { _positionFloat.X = Camera.WorldWidth - halfSize.X; if (_velocity.X > 0) _velocity.X = 0; }

            if (_positionFloat.Y < halfSize.Y)
            { _positionFloat.Y = halfSize.Y; if (_velocity.Y < 0) _velocity.Y = 0; }
            else if (_positionFloat.Y > Camera.WorldHeight - halfSize.Y)
            { _positionFloat.Y = Camera.WorldHeight - halfSize.Y; if (_velocity.Y > 0) _velocity.Y = 0; }

            _rectangleCollider.shape.Location = (_positionFloat - halfSize).ToPoint();

            // ── Turret aiming ─────────────────────────────────────────────────
            // Mouse reports screen coords — convert to world coords for correct aiming
            Camera camera = GameManager.GetGameManager().Camera;
            Vector2 mouseWorld = camera.ScreenToWorld(
                inputManager.CurrentMouseState.Position.ToVector2());

            Vector2 shipCenter = _rectangleCollider.shape.Center.ToVector2();
            Vector2 aimDirection = LinePieceCollider.GetDirection(shipCenter, mouseWorld);
            _turretRotation = LinePieceCollider.GetAngle(aimDirection);

            // ── Shooting ──────────────────────────────────────────────────────
            if (inputManager.LeftMousePress())
            {
                Vector2 turretOrigin = _base_turret.Bounds.Size.ToVector2() / 2f;
                Vector2 turretExit = shipCenter + aimDirection * turretOrigin.Y;

                if (_buffTimer <= 0)
                {
                    GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
                }
                else
                {
                    Vector2 localMuzzleOffset = new Vector2(0f, -turretOrigin.Y);
                    GameManager.GetGameManager().AddGameObject(
                        new Laser(this, localMuzzleOffset, -Vector2.UnitY, 400f));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_buffTimer > 0)
                _buffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 shipCenter = _rectangleCollider.shape.Center.ToVector2();
            Vector2 shipOrigin = _ship_body.Bounds.Size.ToVector2() / 2f;
            spriteBatch.Draw(_ship_body, shipCenter, null, Color.White,
                             _shipRotation, shipOrigin, 1f, SpriteEffects.None, 0);

            Vector2 turretOrigin = _base_turret.Bounds.Size.ToVector2() / 2f;
            Texture2D turret = _buffTimer > 0 ? _laser_turret : _base_turret;
            spriteBatch.Draw(turret, shipCenter, null, Color.White,
                             _turretRotation, turretOrigin, 1f, SpriteEffects.None, 0);

            base.Draw(gameTime, spriteBatch);
        }

        public void Buff() => _buffTimer = _buffDuration;

        public Rectangle GetPosition() => _rectangleCollider.shape;
        public Vector2 GetTurretWorldPosition() => _rectangleCollider.shape.Center.ToVector2();
        public float GetTurretWorldRotation() => _turretRotation;
    }
}
