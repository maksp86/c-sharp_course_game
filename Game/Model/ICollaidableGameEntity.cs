using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace JABEUP_Game.Game.Model
{
	public enum ColliderType
	{
		None,
		Strong,
		StrongPlayer,
		Transparent
	}
	public interface ICollaidableGameEntity : IDrawableGameEntity
	{
		public BoundingBox Collider { get; }
		public Vector3 Position { get; }
		public Vector3 Velocity { get; }
		public ColliderType ColliderType { get; }

		public void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime);
	}
}
