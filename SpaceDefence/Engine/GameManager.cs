using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class GameManager
    {
        private static GameManager gameManager;

        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        private ContentManager _content;

        public Random RNG { get; private set; }
        public Ship Player { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }
        public Camera Camera { get; private set; }
        public Background Background { get; private set; }
        public SpawnManager SpawnManager { get; private set; }

        public Action OnGameOver;

        public static GameManager GetGameManager()
        {
            if (gameManager == null)
                gameManager = new GameManager();
            return gameManager;
        }

        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            InputManager = new InputManager();
            RNG = new Random();
            Background = new Background();
            SpawnManager = new SpawnManager();
        }

        public void Initialize(ContentManager content, Game game, Ship player)
        {
            Game = game;
            _content = content;
            Player = player;
            Camera = new Camera(game.GraphicsDevice.Viewport);
        }

        public void Load(ContentManager content)
        {
            Background.Load(content);
            foreach (GameObject go in _gameObjects)
                go.Load(content);
        }

        public void HandleInput(InputManager inputManager)
        {
            foreach (GameObject go in _gameObjects)
                go.HandleInput(this.InputManager);
        }

        public void CheckCollision()
        {
            for (int i = 0; i < _gameObjects.Count; i++)
                for (int j = i + 1; j < _gameObjects.Count; j++)
                {
                    if (_gameObjects[i].CheckCollision(_gameObjects[j]))
                    {
                        _gameObjects[i].OnCollision(_gameObjects[j]);
                        _gameObjects[j].OnCollision(_gameObjects[i]);
                    }
                }
        }

        public void Update(GameTime gameTime)
        {
            InputManager.Update();
            HandleInput(InputManager);

            foreach (GameObject go in _gameObjects)
                go.Update(gameTime);

            CheckCollision();

            // Ramp up difficulty over time
            SpawnManager.Update(gameTime);

            foreach (GameObject go in _toBeAdded)
            {
                go.Load(_content);
                _gameObjects.Add(go);
            }
            _toBeAdded.Clear();

            foreach (GameObject go in _toBeRemoved)
            {
                go.Destroy();
                _gameObjects.Remove(go);
            }
            _toBeRemoved.Clear();

            if (Player != null)
                Camera.Follow(Player.GetPosition().Center.ToVector2());
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: Camera.GetTransform());
            Background.Draw(spriteBatch, Camera);
            foreach (GameObject go in _gameObjects)
                go.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public void AddGameObject(GameObject gameObject) => _toBeAdded.Add(gameObject);
        public void RemoveGameObject(GameObject gameObject) => _toBeRemoved.Add(gameObject);

        public void ClearGameObjects()
        {
            foreach (GameObject go in _gameObjects) go.Destroy();
            _gameObjects.Clear();
            _toBeAdded.Clear();
            _toBeRemoved.Clear();
        }

        public void GameOver() => OnGameOver?.Invoke();

        public Vector2 RandomScreenLocation()
        {
            return new Vector2(
                RNG.Next(0, Camera.WorldWidth),
                RNG.Next(0, Camera.WorldHeight));
        }
    }
}
