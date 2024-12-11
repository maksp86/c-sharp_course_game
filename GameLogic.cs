using JABEUP_Game.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace JABEUP_Game
{
	public class GameLogic : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private KeyboardState keyboardState;

		GameEnvironment _environment;

		public GameLogic()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 720;

			IsMouseVisible = false;
			Window.AllowUserResizing = false;
			Window.ClientSizeChanged += OnResize;
			_environment = new GameEnvironment(Services, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
		}

		private void OnResize(object sender, EventArgs e)
		{
			if ((_graphics.PreferredBackBufferWidth != _graphics.GraphicsDevice.Viewport.Width) ||
				(_graphics.PreferredBackBufferHeight != _graphics.GraphicsDevice.Viewport.Height))
			{
				_graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Viewport.Height;
				_graphics.PreferredBackBufferWidth = (int)(_graphics.PreferredBackBufferHeight / 9.0 * 16.0);
				_graphics.ApplyChanges();
				_environment.SetViewPort(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
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
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_environment.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			keyboardState = Keyboard.GetState();

			if (keyboardState.IsKeyDown(Keys.Escape))
				Exit();

			_environment.Update(gameTime, keyboardState);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			_environment.Draw(gameTime, _spriteBatch);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
