using JABEUP_Game.Game;
using JABEUP_Game.Game.Controller;
using JABEUP_Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using System;

namespace JABEUP_Game
{
	public class GameLogic : Microsoft.Xna.Framework.Game
	{
		public static readonly string BaseName = "JABEUP";

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private KeyboardState keyboardState;

		public static Texture2D DefaultRectangle;

		GameStateController _gameStateController;
		GameEnvironment _environment;
		GameMenu _gameMenu;

		SaveController _saveEngine;

		public static bool DebugDraw => _debugDraw;
		private static bool _debugDraw = false;

		public static readonly Rectangle BaseViewPort = new Rectangle(0, 0, 1280, 720);
		Vector2 ScaleVector = Vector2.One;

		private bool IsFullScreen;
		private Viewport sizeBeforeFullscreen;
		TimeSpan lastFullscreenChange = TimeSpan.Zero;

		public GameLogic()
		{
			this.Exiting += GameLogic_Exiting;
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = BaseViewPort.Width,
				PreferredBackBufferHeight = BaseViewPort.Height
			};

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += OnResize;

			MyraEnvironment.Game = this;

			_saveEngine = new SaveController();
			_gameStateController = new GameStateController();
			_gameStateController.StateChanged += OnGameStateChanged;
			_environment = new GameEnvironment(Services, _saveEngine, _gameStateController);
			_gameMenu = new GameMenu(_gameStateController, _saveEngine);
		}

		private void OnGameStateChanged(object sender, GameStateChangedEventArgs e)
		{
			IsMouseVisible = e.newValue != GameState.Game;
			switch (e.newValue)
			{
				case GameState.Game:
					Window.Title = BaseName + " - Playing";
					break;
				case GameState.DeadMenu:
					Window.Title = BaseName + " - Dead";
					break;
				case GameState.PauseMenu:
					Window.Title = BaseName + " - Paused";
					break;

				default:
					Window.Title = BaseName;
					break;
			}
		}

		private void UpdateHighScore()
		{
			if (_saveEngine.CurrentData.HighScore < _gameStateController.Score)
				_saveEngine.CurrentData.HighScore = _gameStateController.Score;
		}

		private void GameLogic_Exiting(object sender, EventArgs e)
		{
			UpdateHighScore();
			_saveEngine.Save();
		}

		private void OnResize(object sender, EventArgs e)
		{
			if ((_graphics.PreferredBackBufferWidth != _graphics.GraphicsDevice.Viewport.Width) ||
				(_graphics.PreferredBackBufferHeight != _graphics.GraphicsDevice.Viewport.Height))
			{
				_graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Viewport.Height;
				_graphics.PreferredBackBufferWidth = (int)(_graphics.PreferredBackBufferHeight / 9.0 * 16.0);
				_graphics.ApplyChanges();

				ScaleVector = new Vector2((float)_graphics.PreferredBackBufferWidth / BaseViewPort.Width, (float)_graphics.PreferredBackBufferHeight / BaseViewPort.Height);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			_graphics.ApplyChanges();

			_environment.Initialize();

			Window.Title = BaseName;
			_gameStateController.SetGameState(this, GameState.Menu);
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			_saveEngine.Load();
			_debugDraw = _saveEngine.CurrentData.Options.DebugDraw;

			_spriteBatch = new SpriteBatch(GraphicsDevice);

			DefaultRectangle = new Texture2D(GraphicsDevice, 1, 1);
			DefaultRectangle.SetData(new[] { Color.White });

			_environment.LoadContent();
			_gameMenu.LoadContent(null);
		}

		protected override void Update(GameTime gameTime)
		{
			keyboardState = Keyboard.GetState();

			if (keyboardState.IsKeyDown(Keys.F11) && gameTime.TotalGameTime.Subtract(lastFullscreenChange).TotalSeconds > 0.5d)
			{
				if (IsFullScreen)
				{
					_graphics.GraphicsDevice.Viewport = sizeBeforeFullscreen;
					Window.IsBorderless = false;
				}
				else
				{
					sizeBeforeFullscreen = _graphics.GraphicsDevice.Viewport;
					Window.IsBorderless = true;
					Viewport newViewPort = _graphics.GraphicsDevice.Viewport;

					newViewPort.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					newViewPort.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

					_graphics.GraphicsDevice.Viewport = newViewPort;

				}
				OnResize(null, null);
				IsFullScreen = !IsFullScreen;
				lastFullscreenChange = gameTime.TotalGameTime;
			}


			switch (_gameStateController.GameState)
			{
				case GameState.Menu:
				case GameState.PauseMenu:
				case GameState.DeadMenu:
					_gameMenu.Update(gameTime, keyboardState, null);
					break;
				case GameState.Game:
					if (keyboardState.IsKeyDown(Keys.Escape))
					{
						UpdateHighScore();
						_gameStateController.SetGameState(this, GameState.PauseMenu);
					}
					_environment.Update(gameTime, keyboardState);
					break;

			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			switch (_gameStateController.GameState)
			{
				case GameState.Menu:
				case GameState.PauseMenu:
				case GameState.DeadMenu:
					_gameMenu.Draw(gameTime, _spriteBatch, ScaleVector, 0f);
					break;

				case GameState.Game:
					_environment.Draw(gameTime, _spriteBatch, ScaleVector);
					break;
			}

			base.Draw(gameTime);
		}
	}
}
