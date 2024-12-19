using JABEUP_Game.Game.Controller;
using JABEUP_Game.Game.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace JABEUP_Game.Game.View
{
	public class EnvironmentRenderer : IDrawableGameEntity
	{
		BackgroundController backgroundController;
		public float CameraOffsetX => cameraOffsetX;

		private float cameraOffsetX = 0f;
		float currentMovement = 0;

		ProgressBarRenderer playerHealthBar;


		public EnvironmentRenderer()
		{
			backgroundController = new BackgroundController();
			playerHealthBar = new ProgressBarRenderer();
		}

		public void Initialize()
		{
			cameraOffsetX = 0f;
			backgroundController.Initialize();
		}

		public void LoadContent(ContentManager contentManager)
		{
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Sprites/Background_Mountains/1"), 0.01f, 0.0f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Sprites/Background_Mountains/2"), 0.02f, 0.2f, -50.0f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Sprites/Background_Mountains/3"), 0.03f, 0.5f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Sprites/Background_Mountains/4"), 0.04f, 0.2f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Sprites/Background_Mountains/5"), 0.05f, 1.0f));
			playerHealthBar.LoadContent(contentManager);
		}

		public void UpdatePlayerData(PlayerClass player)
		{
			playerHealthBar.UpdateValue(player.HP, player.MaxHP, new Vector3(cameraOffsetX + GameLogic.BaseViewPort.Width * 0.05f, GameLogic.BaseViewPort.Height * 0.05f, 0));
			playerHealthBar.SetScale(1.5f);

			if ((player.Position.X - CameraOffsetX) > (GameLogic.BaseViewPort.Width / 3f))
				currentMovement = (Math.Max(100, player.Velocity.X / 2f));
			else
				currentMovement = 0;
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController)
		{
			cameraOffsetX += (currentMovement) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			backgroundController.Update(-currentMovement, gameTime);

			playerHealthBar.Update(gameTime, keyboardState, safeZoneController);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float baseOffsetX)
		{
			backgroundController.Draw(spriteBatch, scaleVector);

			playerHealthBar.Draw(gameTime, spriteBatch, scaleVector, cameraOffsetX);
		}
	}
}
