﻿using JABEUP_Game.Game;
using JABEUP_Game.Game.Controller;
using JABEUP_Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using System;

namespace JABEUP_Game
{
	public class GameLogic : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private KeyboardState keyboardState;

		public static Texture2D DefaultRectangle;

		GameStateController _gameStateModel;
		GameEnvironment _environment;
		GameMenu _gameMenu;

		SaveController _saveEngine;

		public static bool DebugDraw => _debugDraw;
		private static bool _debugDraw = false;

		public static readonly Rectangle BaseViewPort = new Rectangle(0, 0, 1280, 720);
		Vector2 ScaleVector = Vector2.One;

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
			_gameStateModel = new GameStateController();
			_environment = new GameEnvironment(Services, _saveEngine, _gameStateModel);
			_gameMenu = new GameMenu(_gameStateModel, _saveEngine);
		}

		private void UpdateHighScore()
		{
			if (_saveEngine.CurrentData.HighScore < _gameStateModel.Score)
				_saveEngine.CurrentData.HighScore = _gameStateModel.Score;
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

			switch (_gameStateModel.GameState)
			{
				case GameState.Menu:
				case GameState.PauseMenu:
				case GameState.DeadMenu:
					_gameMenu.Update(gameTime, keyboardState, null); break;
				case GameState.Game:
					if (keyboardState.IsKeyDown(Keys.Escape))
					{
						UpdateHighScore();
						_gameStateModel.SetGameState(this, GameState.PauseMenu);
					}
					_environment.Update(gameTime, keyboardState);
					break;

			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			switch (_gameStateModel.GameState)
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
