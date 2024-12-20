﻿using JABEUP_Game.Game.Controller;
using JABEUP_Game.Game.Enemy;
using JABEUP_Game.Game.Model;
using JABEUP_Game.Game.Player;
using JABEUP_Game.Game.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
		FrameCounterController fpsController;

		static Vector2 _playerSpawn = new Vector2(100, 700);

		EnvironmentRenderer environmentRenderer;
		EnvironmentSafeZoneController safeZoneController;

		SoundEffect backgroundMusic;
		SoundController _soundController;

		GameStateController _gameStateModel;

		PlayerClass player;
		TimeSpan swtichToDeadScreenTime;
		Color screenFadeColor;

		SaveController _saveController;

		List<ICollaidableGameEntity> collaidableGameEntities;

		SpriteFont gameFont;
		Vector2 scoreTextPos = new Vector2(GameLogic.BaseViewPort.Width * 0.85f, GameLogic.BaseViewPort.Height * 0.05f);

		public GameEnvironment(IServiceProvider serviceProvider, SaveController saveController, GameStateController gameState)
		{
			fpsController = new FrameCounterController();
			collaidableGameEntities = new List<ICollaidableGameEntity>();
			_saveController = saveController;
			_gameStateModel = gameState;
			_soundController = new SoundController(_saveController);

			player = new PlayerClass(_saveController);
			player.OnDead += OnEntityDead;

			environmentRenderer = new EnvironmentRenderer();
			safeZoneController = new EnvironmentSafeZoneController();

			contentManager = new ContentManager(serviceProvider, "Content");

			_gameStateModel.StateChanged += OnGameStateChanged;
		}

		private void OnGameStateChanged(object sender, GameStateChangedEventArgs e)
		{
			if ((e.oldValue == GameState.DeadMenu || e.oldValue == GameState.Menu)
				&& e.newValue == GameState.Game)
			{
				Initialize();
				_soundController.PlaySound(backgroundMusic, SoundController.SoundChannels.Music);
			}

			if (e.newValue == GameState.PauseMenu || e.newValue == GameState.Game)
				_soundController.SetPauseSound(SoundController.SoundChannels.Music, e.newValue != GameState.Game);
		}

		private void OnEntityDead(object sender, EventArgs e)
		{
			AliveGameEntity deadEntity = (sender as AliveGameEntity);

			if (deadEntity.EntityType == AliveEntityType.Mob)
				_gameStateModel.AddEnemyScore(deadEntity);
			else if (deadEntity.EntityType == AliveEntityType.Player)
			{
				swtichToDeadScreenTime = TimeSpan.FromSeconds(5);
			}
		}

		public void Initialize()
		{
			screenFadeColor = Color.Black;
			screenFadeColor.A = 0;

			_gameStateModel.ClearScore();

			environmentRenderer.Initialize();
			safeZoneController.Initialize();

			collaidableGameEntities.Clear();

			player.Initialize(_playerSpawn);

			collaidableGameEntities.Add(player);

			FloatingTextEntity entryText = new FloatingTextEntity(gameFont, "Hello there\nYou need to move right, okay", 1.5f);
			entryText.Initialize(new Vector2(GameLogic.BaseViewPort.Width * 0.6f, GameLogic.BaseViewPort.Height * 0.6f));

			collaidableGameEntities.Add(entryText);

			entryText = new FloatingTextEntity(gameFont, "Well done!\nBe aware of monsters!", 1.5f);
			entryText.Initialize(new Vector2(GameLogic.BaseViewPort.Width * 1.6f, GameLogic.BaseViewPort.Height * 0.6f));

			collaidableGameEntities.Add(entryText);

			_soundController.StopSound(SoundController.SoundChannels.Music);
		}

		private int _spawnTickTime = 0;

		public void SpawnEntities()
		{
			if (++_spawnTickTime < 10)
				return;
			_spawnTickTime = 0;

			collaidableGameEntities.RemoveAll(e => environmentRenderer.CameraOffsetX - e.Position.X > (e.Collider.Max.X - e.Position.X) * 4f);

			if (collaidableGameEntities.Count(e => e is BushClass
			&& (e.Position.X - environmentRenderer.CameraOffsetX > (GameLogic.BaseViewPort.Width / 4))) < 1)
			{
				for (int i = 0; i < Random.Shared.Next(1,4); i++)
				{
					BushClass newBush = new BushClass(Random.Shared.Next(1, 5).ToString());
					newBush.LoadContent(contentManager);
					newBush.Initialize(new Vector2(environmentRenderer.CameraOffsetX + GameLogic.BaseViewPort.Width + Random.Shared.Next(100, GameLogic.BaseViewPort.Width), Random.Shared.Next(480, 721)));
					collaidableGameEntities.Add(newBush);
				}
			}

			if (collaidableGameEntities.Count(e => e is AliveGameEntity entity && entity.EntityType == AliveEntityType.Mob && (e.Position.X - environmentRenderer.CameraOffsetX > (GameLogic.BaseViewPort.Width / 2))) < 1)
			{
				for (int i = 0; i < Random.Shared.Next(3); i++)
				{
					AliveGameEntity newEnemy;
					if (Random.Shared.Next(2) == 0)
						newEnemy = new EnemyZombieClass();
					else
						newEnemy = new EnemySlimeClass();

					newEnemy.LoadContent(contentManager);
					newEnemy.Initialize(new Vector2(environmentRenderer.CameraOffsetX + GameLogic.BaseViewPort.Width + Random.Shared.Next(100, GameLogic.BaseViewPort.Width), Random.Shared.Next(480, 721)));
					newEnemy.OnDead += OnEntityDead;
					collaidableGameEntities.Add(newEnemy);
				}
			}
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
			if (!player.IsAlive)
			{
				if (swtichToDeadScreenTime > TimeSpan.Zero)
				{
					swtichToDeadScreenTime = swtichToDeadScreenTime.Subtract(gameTime.ElapsedGameTime);
					screenFadeColor.A = Math.Clamp((byte)((1 - swtichToDeadScreenTime.TotalSeconds / 5.0f) * 255), Byte.MinValue, Byte.MaxValue);
				}
				else
					_gameStateModel.SetGameState(this, GameState.DeadMenu);
			}

			SpawnEntities();

			safeZoneController.Update(environmentRenderer.CameraOffsetX);

			foreach (ICollaidableGameEntity entity in collaidableGameEntities
				.Where(e => Math.Abs(e.Position.X - environmentRenderer.CameraOffsetX) > -(GameLogic.BaseViewPort.Width * 1.5)))
			{
				entity.Update(gameTime, keyboardState, safeZoneController);
				entity.UpdateCollisions(collaidableGameEntities, gameTime);
			}

			environmentRenderer.UpdatePlayerData(player);
			environmentRenderer.Update(gameTime, keyboardState, safeZoneController);

			_gameStateModel.UpdateScore(environmentRenderer.CameraOffsetX);
		}

		public void LoadContent()
		{
			LoadContent(contentManager);
		}

		public void LoadContent(ContentManager contentManager)
		{
			environmentRenderer.LoadContent(contentManager);
			player.LoadContent(contentManager);
			gameFont = contentManager.Load<SpriteFont>("GUI/Commodore64Font");
			backgroundMusic = contentManager.Load<SoundEffect>("Sound/Music");
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector)
		{
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			environmentRenderer.Draw(gameTime, spriteBatch, scaleVector, 0);
			safeZoneController.Draw(gameTime, spriteBatch, scaleVector, environmentRenderer.CameraOffsetX);

			foreach (ICollaidableGameEntity entity in collaidableGameEntities
				.Where(e => Math.Abs(e.Position.X - environmentRenderer.CameraOffsetX) < (GameLogic.BaseViewPort.Width))
				.OrderBy(e => e.Position.Y))
			{
				if (GameLogic.DebugDraw)
				{
					spriteBatch.Draw(
				GameLogic.DefaultRectangle,
				new Vector2((entity.Collider.Min.X - environmentRenderer.CameraOffsetX) * scaleVector.X, (entity.Collider.Min.Y + entity.Collider.Min.Z) * scaleVector.Y),
				new Rectangle(0, 0,
					(int)((entity.Collider.Max.X - entity.Collider.Min.X) * scaleVector.Y),
					(int)((entity.Collider.Max.Y - entity.Collider.Min.Y) * scaleVector.Y)),
				Color.Green);
				}

				entity.Draw(gameTime, spriteBatch, scaleVector, environmentRenderer.CameraOffsetX);

			}

			string scoreText = "Score: " + _gameStateModel.Score;
			Vector2 scoreTextOrigin = gameFont.MeasureString(scoreText) / 2;

			spriteBatch.DrawString(gameFont, scoreText, scoreTextPos * scaleVector, Color.White, 0f, scoreTextOrigin, scaleVector.X * 1.5f, SpriteEffects.None, 1f);

			if (!player.IsAlive)
			{
				spriteBatch.Draw(GameLogic.DefaultRectangle, Vector2.Zero, new Rectangle(0, 0, (int)(GameLogic.BaseViewPort.Width * scaleVector.X), (int)(GameLogic.BaseViewPort.Height * scaleVector.Y)), screenFadeColor);
			}

			if (GameLogic.DebugDraw)
			{
				fpsController.Update(gameTime);
				spriteBatch.DrawString(gameFont, $"{Math.Round(fpsController.AverageFramesPerSecond, 2)}FPS", Vector2.One, Color.White);
			}

			spriteBatch.End();
		}
	}
}
