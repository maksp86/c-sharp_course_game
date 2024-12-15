using JABEUP_Game.Game.Controller;
using JABEUP_Game.Game.Model;
using JABEUP_Game.Game.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JABEUP_Game.Game
{
	class BushClass : ICollaidableGameEntity, IDrawableGameEntity
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
			bushCollider = new BoundingBox(position, position + new Vector3(animationIdle.FrameWidth, animationIdle.FrameHeight, animationIdle.FrameHeight / 3));
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
		}

		public void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime)
		{
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
		{
			Vector2 actualPos = new Vector2((position.X - cameraOffsetX) * scaleVector.X, (position.Y + position.Z) * scaleVector.Y);
			float scale = scaleVector.Y * (position.Y / GameLogic.BaseViewPort.Height);

#if DEBUG
			//spriteBatch.Draw(
			//	GameLogic.DebugRectangle,
			//	new Vector2((bushCollider.Min.X - cameraOffsetX) * scaleVector.X, (bushCollider.Min.Y + bushCollider.Min.Z) * scaleVector.Y),
			//	new Rectangle(0, 0, (int)((bushCollider.Max.X - bushCollider.Min.X) * scale), (int)((bushCollider.Max.Y - bushCollider.Min.Y) * scale)),
			//	Color.CadetBlue);
#endif

			animationController.Draw(gameTime, spriteBatch, actualPos, SpriteEffects.None, scale);

		}
	}
}
