using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace JABEUP_Game.Game.Model
{
	public enum AliveEntityType
	{
		Player,
		Mob
	}

	public abstract class AliveGameEntity : ICollaidableGameEntity, IDrawableGameEntity
	{
		public virtual AliveEntityType EntityType { get; }
		public virtual ColliderType ColliderType { get; }


		public bool IsAlive => HP > 0;
		public virtual int MaxHP { get; }
		public int HP => _hp;
		private protected int _hp;

		public BoundingBox Collider => _collider;
		private protected BoundingBox _collider;

		public Vector3 Position => _position;
		private protected Vector3 _position;

		public Vector3 Velocity => _velocity;
		private protected Vector3 _velocity;

		private protected Vector2 _movement;

		public bool IsOnGround => _position.Z == 0;

		public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX);

		public virtual void Initialize(Vector2 withPosition)
		{
			_hp = MaxHP;
			_movement = Vector2.Zero;
			_position = new Vector3(withPosition, 0);
		}

		public virtual void TakeDamage(int damage)
		{
			if (_hp - damage < 0)
				_hp = 0;
			else
				_hp -= damage;
		}

		public abstract void Update(GameTime gameTime, KeyboardState keyboardState);

		public abstract void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime);

		private protected bool isJumping;
		private protected bool wasJumping;
		private protected float jumpTime;

		public const float MoveAcceleration = 13000.0f;
		public const float MaxMoveSpeed = 1750.0f;
		public const float GroundDragFactor = 0.48f;
		public const float AirDragFactor = 0.58f;

		public const float MaxJumpTime = 0.35f;
		public const float JumpLaunchVelocity = -3500.0f;
		public const float GravityAcceleration = 3400.0f;
		public const float MaxFallSpeed = 550.0f;
		public const float JumpControlPower = 0.14f;

		public void DoPhysics(GameTime gameTime, IEnumerable<EnvironmentCollider> environmentColliders)
		{
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Vector3 previousPosition = _position;

			_velocity.X += _movement.X * MoveAcceleration * elapsed;

			_velocity.Y += _movement.Y * MoveAcceleration / 16 * 9 * elapsed;

			_velocity.Z = MathHelper.Clamp(_velocity.Z + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

			_velocity.Z = DoJump(_velocity.Z, gameTime);

			if (IsOnGround)
			{
				_velocity.X *= GroundDragFactor;
				_velocity.Y *= GroundDragFactor;
			}
			else
			{
				_velocity.X *= AirDragFactor;
				_velocity.Y *= AirDragFactor;
			}

			_velocity.X = MathHelper.Clamp(_velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
			_velocity.Y = MathHelper.Clamp(_velocity.Y, -MaxMoveSpeed / 16 * 9, MaxMoveSpeed / 16 * 9);

			_position += _velocity * elapsed;
			_position = new Vector3((float)Math.Round(_position.X), (float)Math.Round(_position.Y), (float)Math.Round(_position.Z));

			HandleEnvironmentCollisions(environmentColliders, previousPosition);

			if (_position.X == previousPosition.X && _movement.X == 0)
				_velocity.X = 0;

			if (_position.Y == previousPosition.Y)
				_velocity.Y = 0;

			if (_position.Z == previousPosition.Z)
				_velocity.Z = 0;
		}

		private float DoJump(float velocityY, GameTime gameTime)
		{
			if (isJumping)
			{
				if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
					jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
					velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
				else
					jumpTime = 0.0f;
			}
			else
				jumpTime = 0.0f;
			wasJumping = isJumping;

			return velocityY;
		}

		private protected virtual void HandleEnvironmentCollisions(IEnumerable<EnvironmentCollider> environmentColliders, Vector3 previousPosition)
		{
			if (_position.Z > 0)
				_position.Z = 0;

			foreach (var collider in environmentColliders)
			{
				if (collider.ColliderType == ColliderType.StrongPlayer && EntityType != AliveEntityType.Player)
					continue;

				if (collider.Collider.Contains(new Vector3(_position.X, _position.Y, 0)) == ContainmentType.Contains)
					_position = previousPosition;

			}

			//if (_position.Y > 720)
			//	_position.Y = 720;
			//else if (_position.Y < 480)
			//	_position.Y = 480;

		}
	}
}
