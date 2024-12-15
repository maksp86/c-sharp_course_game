using JABEUP_Game.Game.Controller;
using JABEUP_Game.Game.Model;
using JABEUP_Game.Game.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace JABEUP_Game.Game.View
{
	public class EnvironmentRenderer
	{
		BackgroundController backgroundController;
		public float CameraOffsetX => cameraOffsetX;

		private float cameraOffsetX = 0f;
		float currentMovement = 0;
		Vector2 _scaleVector = Vector2.One;

		ProgressBarRenderer playerHealthBar;

		List<EnvironmentCollider> colliders = new List<EnvironmentCollider>();

		public EnvironmentRenderer()
		{
			backgroundController = new BackgroundController();
			playerHealthBar = new ProgressBarRenderer();
		}

		public void LoadContent(ContentManager contentManager)
		{
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/1"), 0.01f, 0.0f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/2"), 0.02f, 0.2f, -50.0f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/3"), 0.03f, 0.5f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/4"), 0.04f, 0.2f));
			backgroundController.AddLayer(new Layer(contentManager.Load<Texture2D>("Background_Mountains/1/5"), 0.05f, 1.0f));
			playerHealthBar.LoadContent(contentManager);
		}

		public void UpdatePlayerData(PlayerClass player)
		{
			playerHealthBar.UpdateValue(player.HP, player.MaxHP, new Vector3(cameraOffsetX + GameLogic.BaseViewPort.Width * 0.05f, GameLogic.BaseViewPort.Height * 0.05f, 0));
			playerHealthBar.SetScale(1.5f);

			if ((player.Position.X - CameraOffsetX) > (GameLogic.BaseViewPort.Width / 3f))
				currentMovement = (Math.Max(40, player.Velocity.X / 3f)) * _scaleVector.X;
			else
				currentMovement = 0;
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
			cameraOffsetX += (currentMovement) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			backgroundController.Update(-currentMovement, gameTime);

			playerHealthBar.Update(gameTime, keyboardState);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector)
		{
			_scaleVector = scaleVector;
			backgroundController.Draw(spriteBatch, scaleVector);

			playerHealthBar.Draw(gameTime, spriteBatch, scaleVector, cameraOffsetX);
		}
	}
}
