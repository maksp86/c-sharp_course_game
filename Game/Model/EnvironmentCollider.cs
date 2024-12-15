using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JABEUP_Game.Game.Model
{
	public class EnvironmentCollider : ICollaidableGameEntity
	{
		private Func<EnvironmentCollider, GameTime, KeyboardState, BoundingBox> updateDelegate;

		public BoundingBox Collider => _collider;
		public BoundingBox _collider;

		public Vector3 Position => _position;
		private Vector3 _position;

		public Vector3 Velocity => Vector3.Zero;

		public ColliderType ColliderType => ColliderType.Strong;
		private ColliderType _colliderType;

		public EnvironmentCollider(BoundingBox collider, ColliderType colliderType)
		{
			_collider = collider;
			_position = _collider.Min;
			_colliderType = colliderType;
		}

		public EnvironmentCollider(Func<EnvironmentCollider, GameTime, KeyboardState, BoundingBox> update, ColliderType colliderType)
		{
			updateDelegate = update;
			_colliderType = colliderType;
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX) { }

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
			if (updateDelegate != null)
			{
				_collider = updateDelegate(this, gameTime, keyboardState);
				_position = _collider.Min;
			}
		}

		public void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime) { }
	}
}
