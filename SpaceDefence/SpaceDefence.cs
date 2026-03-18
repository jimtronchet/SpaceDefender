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
        private GameOverScreen _gameOverScreen;

        public SpaceDefence()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;

            // Set the size of the screen
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Initialize the GameManager
            _gameManager = GameManager.GetGameManager();
            _gameOverScreen = new GameOverScreen(GraphicsDevice, RestartGame);
            _gameManager.OnGameOver = () => _gameOverScreen.Activate();

            base.Initialize();

            StartGame();
        }

        private void StartGame()
        {
            // Place the player at the center of the screen
            Ship player = new Ship(new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

            // Add the starting objects to the GameManager
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
            _gameOverScreen.SetFonts(fontLarge, fontSmall);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _gameOverScreen.Update(gameTime);

            // Freeze the game world while the death screen is showing
            if (!_gameOverScreen.IsActive)
                _gameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _gameManager.Draw(gameTime, _spriteBatch);
            _gameOverScreen.Draw(gameTime);  // drawn on top
            base.Draw(gameTime);
        }
    }
}
