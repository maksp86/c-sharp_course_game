using JABEUP_Game.Game.Controller;
using JABEUP_Game.Game.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace JABEUP_Game.Game
{
	class BushClass : ICollaidableGameEntity
	{
		private Animation animationIdle;
		private AnimationController animationController;

		public ColliderType ColliderType => ColliderType.None;
		public BoundingBox Collider => bushCollider;
		private BoundingBox bushCollider;

		public Vector3 Velocity => Vector3.Zero;

		public Vector3 Position => position;
		private Vector3 position;

		private string _bushType;

		private bool needToCheckPosition;

		public BushClass(string bushType = "1")
		{
			_bushType = bushType;
		}

		public void Initialize(Vector2 spawnPosition)
		{
			Reset(spawnPosition);
		}

		public void LoadContent(ContentManager content)
		{
			animationIdle = new Animation(content.Load<Texture2D>("Bush/Bush1_" + _bushType), 1f, false);
		}

		public void Reset(Vector2 withPosition)
		{
			animationController.PlayAnimation(animationIdle);
			position = new Vector3(withPosition.X, withPosition.Y, 0);
			needToCheckPosition = true;
			bushCollider = new BoundingBox();
			UpdateCollider();
		}

		private void UpdateCollider()
		{
			bushCollider.Min = new Vector3(position.X, position.Y - animationIdle.FrameHeight, position.Z);
			bushCollider.Max = bushCollider.Min + new Vector3(animationIdle.FrameWidth * 1.5f, animationIdle.FrameHeight, 0);
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController)
		{
			if (!needToCheckPosition)
				return;

			if (!safeZoneController.IsInSafeZone(bushCollider.Max) || !safeZoneController.IsInSafeZone(position))
			{
				position.Y += 1;
				UpdateCollider();
			}
			else
				needToCheckPosition = false;
		}

		public void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime)
		{
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
		{
			Vector2 actualPos = new Vector2((position.X - cameraOffsetX) * scaleVector.X, (position.Y + position.Z) * scaleVector.Y);
			float scale = scaleVector.Y * (position.Y / GameLogic.BaseViewPort.Height);

			animationController.Draw(gameTime, spriteBatch, actualPos, SpriteEffects.None, scale);

		}
	}
}
