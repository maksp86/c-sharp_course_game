using JABEUP_Game.Game.Controller;
using JABEUP_Game.Game.Enemy;
using JABEUP_Game.Game.Model;
using JABEUP_Game.Game.Player;
using JABEUP_Game.Game.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JABEUP_Game.Game
{
	class GameEnvironment
	{
		ContentManager contentManager;

		static Vector2 _playerSpawn = new Vector2(100, 500);


		EnvironmentRenderer environmentRenderer;
		PlayerClass player;
		EnemyClass enemy;
		BushClass bush;
		SaveController _saveController;

		List<ICollaidableGameEntity> collaidableGameEntities;

		SpriteFont gameFont;
		Vector2 scoreTextPos = new Vector2(GameLogic.BaseViewPort.Width * 0.85f, GameLogic.BaseViewPort.Height * 0.05f);

		public int GameScore => (int)Math.Ceiling(environmentRenderer.CameraOffsetX / (GameLogic.BaseViewPort.Width / 3f));

		public GameEnvironment(IServiceProvider serviceProvider, SaveController saveController)
		{
			collaidableGameEntities = new List<ICollaidableGameEntity>();
			_saveController = saveController;

			enemy = new EnemyClass();
			player = new PlayerClass(_saveController);
			bush = new BushClass("4");

			environmentRenderer = new EnvironmentRenderer();

			contentManager = new ContentManager(serviceProvider, "Content");
		}

		public void Initialize()
		{
			player.Initialize(_playerSpawn);
			enemy.Initialize(new Vector2(1000, GameLogic.BaseViewPort.Height));
			bush.Initialize(new Vector2(1000, 620));

			collaidableGameEntities.Add(player);
			//collaidableGameEntities.Add(enemy);
			collaidableGameEntities.Add(bush);

			collaidableGameEntities.Add(new EnvironmentCollider(
				new BoundingBox(
					new Vector3(100, 100, 0),
					new Vector3(500, 200, 0)
					), ColliderType.Strong));

			collaidableGameEntities.Add(new EnvironmentCollider((c, gt, ks) =>
			{
				return new BoundingBox(
						new Vector3(environmentRenderer.CameraOffsetX, GameLogic.BaseViewPort.Height - 10, -10),
						new Vector3(environmentRenderer.CameraOffsetX + GameLogic.BaseViewPort.Width, GameLogic.BaseViewPort.Height + 100, 10)
						);
			}, ColliderType.Strong));

			collaidableGameEntities.Add(new EnvironmentCollider((c, gt, ks) =>
			{
				return new BoundingBox(
						new Vector3(environmentRenderer.CameraOffsetX, 0, -10),
						new Vector3(environmentRenderer.CameraOffsetX + 10, GameLogic.BaseViewPort.Height, 10)
						);
			}, ColliderType.StrongPlayer));
		}

		public void SpawnEntities()
		{
			collaidableGameEntities.RemoveAll(e => environmentRenderer.CameraOffsetX - e.Position.X > (e.Collider.Max.X - e.Position.X) * 1.5f);

			if (collaidableGameEntities.Count(e => e is BushClass
			&& (e.Position.X - environmentRenderer.CameraOffsetX > (GameLogic.BaseViewPort.Width / 2))) < 1)
			{
				BushClass newBush = new BushClass(Random.Shared.Next(1, 5).ToString());
				newBush.LoadContent(contentManager);
				newBush.Initialize(new Vector2(environmentRenderer.CameraOffsetX + GameLogic.BaseViewPort.Width + Random.Shared.Next(100, 300), Random.Shared.Next(600, 721)));
				collaidableGameEntities.Add(newBush);
			}

			//if (collaidableGameEntities.Count(e => e is EnemyClass && (e.Position.X - environmentRenderer.CameraOffsetX > (GameLogic.BaseViewPort.Width / 2))) < 1)
			//{
			//	EnemyClass newEnemy = new EnemyClass();
			//	newEnemy.LoadContent(contentManager);
			//	newEnemy.Initialize(new Vector2(environmentRenderer.CameraOffsetX + GameLogic.BaseViewPort.Width + Random.Shared.Next(100, 300), Random.Shared.Next(600, 721)));
			//	collaidableGameEntities.Add(newEnemy);
			//}
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
			SpawnEntities();

			foreach (ICollaidableGameEntity entity in collaidableGameEntities)
			{
				entity.Update(gameTime, keyboardState);
				entity.UpdateCollisions(collaidableGameEntities, gameTime);
			}

			environmentRenderer.UpdatePlayerData(player);
			environmentRenderer.Update(gameTime, keyboardState);
		}

		public void LoadContent()
		{
			LoadContent(contentManager);
		}

		public void LoadContent(ContentManager contentManager)
		{
			environmentRenderer.LoadContent(contentManager);
			player.LoadContent(contentManager);
			enemy.LoadContent(contentManager);
			bush.LoadContent(contentManager);
			gameFont = contentManager.Load<SpriteFont>("GUI/Commodore64Font");
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector)
		{
			environmentRenderer.Draw(gameTime, spriteBatch, scaleVector);

			foreach (ICollaidableGameEntity entity in collaidableGameEntities
				.Where(e => e.Position.X - environmentRenderer.CameraOffsetX > -(GameLogic.BaseViewPort.Width))
				.OrderBy(e => e.Position.Y))
			{
#if DEBUG
				spriteBatch.Draw(
				GameLogic.DebugRectangle,
				new Vector2((entity.Collider.Min.X - environmentRenderer.CameraOffsetX) * scaleVector.X, (entity.Collider.Min.Y) * scaleVector.Y),
				new Rectangle(0, 0,
					(int)((entity.Collider.Max.X - entity.Collider.Min.X) * scaleVector.Y),
					(int)((entity.Collider.Max.Y - entity.Collider.Min.Y) * scaleVector.Y)),
				Color.CadetBlue);
#endif

				entity.Draw(gameTime, spriteBatch, scaleVector, environmentRenderer.CameraOffsetX);

				if (entity is AliveGameEntity aliveEntity && aliveEntity.IsAlive)
				{

				}
			}

			string scoreText = "Score: " + GameScore;
			Vector2 scoreTextOrigin = gameFont.MeasureString(scoreText) / 2;

			spriteBatch.DrawString(gameFont, scoreText, scoreTextPos * scaleVector, Color.White, 0f, scoreTextOrigin, scaleVector.X * 1.5f, SpriteEffects.None, 1f);
		}
	}
}
