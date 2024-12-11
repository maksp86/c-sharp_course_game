using JABEUP_Game.Game.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace JABEUP_Game.Game
{
	class GameEnvironment : IGameEntity
	{
		ContentManager contentManager;

		private Rectangle ViewPort;

		static Vector2 _playerSpawn = new Vector2(100, 0);
		BackgroundManager backgroundManager;
		PlayerClass player;


		public GameEnvironment(IServiceProvider serviceProvider, Rectangle viewPort)
		{
			player = new PlayerClass();
			backgroundManager = new BackgroundManager();
			contentManager = new ContentManager(serviceProvider, "Content");
			ViewPort = viewPort;
		}

		public void Initialize()
		{
			player.Initialize(_playerSpawn);
			backgroundManager.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/1"), 0.0f, 0.0f));
			backgroundManager.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/2"), 0.1f, 0.2f, -50.0f));
			backgroundManager.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/3"), 0.2f, 0.5f));
			backgroundManager.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/4"), 0.3f, 0.2f));
			backgroundManager.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/5"), 0.4f, 1.0f));
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
			player.Update(gameTime, keyboardState);
			backgroundManager.Update(-(player.Velocity.X / 3), gameTime);
		}

		public void LoadContent()
		{
			LoadContent(contentManager);
		}

		public void LoadContent(ContentManager contentManager)
		{
			player.LoadContent(contentManager);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			backgroundManager.Draw(spriteBatch, ViewPort);
			player.Draw(gameTime, spriteBatch);
		}

		public void SetViewPort(Rectangle viewPort)
		{
			ViewPort = viewPort;
		}
	}
}
