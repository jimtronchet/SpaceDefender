using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class SpaceDefence : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;

        private StartScreen _startScreen;
        private PauseScreen _pauseScreen;
        private GameOverScreen _gameOverScreen;

        private KeyboardState _prevKeys;

        public SpaceDefence()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gameManager = GameManager.GetGameManager();

            // All screens must exist before base.Initialize() because
            // MonoGame calls LoadContent() from inside base.Initialize().
            _startScreen = new StartScreen(GraphicsDevice, onStart: StartGame, onQuit: Exit);
            _pauseScreen = new PauseScreen(GraphicsDevice, onContinue: () => { }, onQuit: Exit);
            _gameOverScreen = new GameOverScreen(GraphicsDevice, onRespawn: RestartGame);

            _gameManager.OnGameOver = () => _gameOverScreen.Activate();

            base.Initialize();
        }

        private void StartGame()
        {
            Ship player = new Ship(new Point(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2));

            _gameManager.Initialize(Content, this, player);
            _gameManager.AddGameObject(player);
            _gameManager.AddGameObject(new Alien());
            _gameManager.AddGameObject(new Supply());
        }

        private void RestartGame()
        {
            _gameManager.ClearGameObjects();
            StartGame();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameManager.Load(Content);

            SpriteFont fontLarge = Content.Load<SpriteFont>("YouDiedLarge");
            SpriteFont fontSmall = Content.Load<SpriteFont>("YouDiedSmall");

            // Start and pause only need the small font
            _startScreen.SetFonts(fontSmall);
            _pauseScreen.SetFonts(fontSmall);
            _gameOverScreen.SetFonts(fontLarge, fontSmall);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // Start screen blocks everything else
            if (_startScreen.IsActive)
            {
                _startScreen.Update(gameTime);
                _prevKeys = keys;
                base.Update(gameTime);
                return;
            }

            // Game over blocks everything else
            if (_gameOverScreen.IsActive)
            {
                _gameOverScreen.Update(gameTime);
                _prevKeys = keys;
                base.Update(gameTime);
                return;
            }

            // Escape toggles pause (single press — only when not on another screen)
            if (keys.IsKeyDown(Keys.Escape) && !_prevKeys.IsKeyDown(Keys.Escape)
                && !_pauseScreen.IsActive)
            {
                _pauseScreen.Toggle();
            }

            // Pause: world is drawn but not updated (handled in Draw)
            if (_pauseScreen.IsActive)
            {
                _pauseScreen.Update(gameTime);
                _prevKeys = keys;
                base.Update(gameTime);
                return;
            }

            // Normal gameplay
            _gameManager.Update(gameTime);

            _prevKeys = keys;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw the game world whenever the start screen is not showing
            if (!_startScreen.IsActive)
                _gameManager.Draw(gameTime, _spriteBatch);

            // Overlays on top (each checks IsActive internally)
            _startScreen.Draw(gameTime);
            _pauseScreen.Draw(gameTime);
            _gameOverScreen.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
