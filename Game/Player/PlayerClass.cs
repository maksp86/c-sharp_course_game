using JABEUP_Game.Game.Controller;
using JABEUP_Game.Game.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JABEUP_Game.Game.Player
{
	public class PlayerClass : AliveGameEntity
	{
		private Animation animationIdle, animationRun, animationRunAttack, animationJump, animationAttack, animationAttackDefend, animationDefend, animationHurt, animationDeath;
		private AnimationController animationObj;
		private SpriteEffects flip = SpriteEffects.None;

		public override AliveEntityType EntityType => AliveEntityType.Player;
		public override ColliderType ColliderType => ColliderType.Strong;
		public override int MaxHP => 500;

		private BoundingBox swordCollider;

		private bool isHurt;
		private TimeSpan hurtEndTime;

		private int nextDamage;
		private bool isDefending;
		private bool isAttacking;
		private bool attackHappened;
		private TimeSpan attackEndTime;
		private Animation attackAnimation;

		private SaveController _saveController;

		public PlayerClass(SaveController saveController)
		{
			_velocity = Vector3.Zero;
			_position = Vector3.Zero;
			_movement = Vector2.Zero;
			_saveController = saveController;
		}

		public override void LoadContent(ContentManager content)
		{
			animationIdle = new Animation(content.Load<Texture2D>("Sprites/Knight/Idle"), 0.2f, true);
			animationRun = new Animation(content.Load<Texture2D>("Sprites/Knight/Run"), 0.2f, true);
			animationJump = new Animation(content.Load<Texture2D>("Sprites/Knight/Jump"), 0.2f, false);
			animationRunAttack = new Animation(content.Load<Texture2D>("Sprites/Knight/Attack_Run"), 0.15f, false);
			animationAttack = new Animation(content.Load<Texture2D>("Sprites/Knight/Attack"), 0.15f, false);
			animationAttackDefend = new Animation(content.Load<Texture2D>("Sprites/Knight/Attack_Defend"), 0.15f, false);
			animationDefend = new Animation(content.Load<Texture2D>("Sprites/Knight/Defend"), 0.2f, true);
			animationHurt = new Animation(content.Load<Texture2D>("Sprites/Knight/Hurt"), 0.2f, false);
			animationDeath = new Animation(content.Load<Texture2D>("Sprites/Knight/Dead"), 0.2f, false);
		}

		public override void Initialize(Vector2 withPosition)
		{
			base.Initialize(withPosition);

			isAttacking = false;
			isDefending = false;
			isHurt = false;
			hurtEndTime = TimeSpan.Zero;
			attackEndTime = TimeSpan.Zero;

			animationObj.PlayAnimation(animationIdle);
			_collider = new BoundingBox(_position, _position + new Vector3(animationIdle.FrameHeight, animationIdle.FrameHeight, animationIdle.FrameHeight / 3));
			swordCollider = new BoundingBox(Vector3.Zero, Vector3.Zero);
		}

		public override void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController)
		{
			if (!IsAlive)
				return;

			_handleActions(keyboardState, gameTime);
			_handleMovement(keyboardState);

			DoPhysics(gameTime, safeZoneController);

			_collider.Min = new Vector3(animationIdle.FrameWidth * 0.2f + _position.X, _position.Y - animationIdle.FrameHeight, _position.Z);
			_collider.Max = _collider.Min + new Vector3(animationIdle.FrameWidth * 0.6f, animationIdle.FrameHeight, 0);


			if (isHurt)
			{
				animationObj.PlayAnimation(animationHurt);

				if (hurtEndTime == TimeSpan.Zero)
					hurtEndTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(animationHurt.FrameTime * (animationHurt.FrameCount * 2));

				if (gameTime.TotalGameTime >= hurtEndTime)
				{
					hurtEndTime = TimeSpan.Zero;
					isHurt = false;
				}
			}
			else if (!isAttacking && IsAlive)
			{
				if (IsOnGround)
				{
					if (isDefending)
						animationObj.PlayAnimation(animationDefend);
					else if (_velocity.X == 0 && _movement.X == 0 && _movement.Y == 0)
						animationObj.PlayAnimation(animationIdle);
					else
						animationObj.PlayAnimation(animationRun);
				}
				else animationObj.PlayAnimation(animationJump);
			}
			_movement = Vector2.Zero;
			isJumping = false;
		}

		public override void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime)
		{
			if (isAttacking)
			{
				if (!attackHappened && animationObj.FrameIndex >= (animationObj.Animation.FrameCount * 2f / 3f))
				{
					attackHappened = true;
					if (flip == SpriteEffects.FlipHorizontally)
						swordCollider = new BoundingBox(new Vector3(_collider.Min.X - animationIdle.FrameHeight / 3, _collider.Min.Y, _collider.Min.Z),
							new Vector3(_collider.Min.X, _collider.Max.Y, _collider.Max.Z));
					else
						swordCollider = new BoundingBox(new Vector3(_collider.Max.X, _collider.Min.Y, _collider.Min.Z),
							new Vector3(_collider.Max.X + animationIdle.FrameHeight / 3, _collider.Max.Y, _collider.Max.Z));

					foreach (AliveGameEntity aliveEntity in collaidables.Where(c => c != this && c is AliveGameEntity && (c as AliveGameEntity).Collider.Intersects(swordCollider)))
					{
						aliveEntity.TakeDamage(nextDamage);
					}
				}
				if (gameTime.TotalGameTime >= attackEndTime)
				{
					isAttacking = false;
					attackHappened = false;
				}
				animationObj.PlayAnimation(attackAnimation);
			}
		}

		private void _handleActions(KeyboardState keyboardState, GameTime gameTime)
		{
			if (keyboardState.IsKeyDown(_saveController.CurrentData.Options.KeyBindings["Attack"]) && !isAttacking && gameTime.TotalGameTime - attackEndTime > TimeSpan.FromMilliseconds(500))
			{
				isAttacking = true;
				if (_velocity.X != 0 || _velocity.Y != 0)
				{
					attackAnimation = animationRunAttack;
					nextDamage = 60;
				}
				else if (isDefending)
				{
					attackAnimation = animationAttackDefend;
					nextDamage = 40;
				}
				else
				{
					_velocity.X += 50;
					attackAnimation = animationAttack;
					nextDamage = 50;
				}
				attackEndTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(attackAnimation.FrameTime * attackAnimation.FrameCount);
			}

			isDefending = !isJumping && keyboardState.IsKeyDown(Keys.LeftShift);
		}

		private void _handleMovement(KeyboardState keyboardState)
		{
			if (isDefending)
				return;

			if (keyboardState.IsKeyDown(_saveController.CurrentData.Options.KeyBindings["Left"]))
				_movement.X = -1.0f;
			else if (keyboardState.IsKeyDown(_saveController.CurrentData.Options.KeyBindings["Right"]))
				_movement.X = 1.0f;

			if (keyboardState.IsKeyDown(_saveController.CurrentData.Options.KeyBindings["Down"]))
				_movement.Y = 1.0f;
			else if (keyboardState.IsKeyDown(_saveController.CurrentData.Options.KeyBindings["Up"]))
				_movement.Y = -1.0f;

			isJumping = keyboardState.IsKeyDown(_saveController.CurrentData.Options.KeyBindings["Jump"]);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
		{
			if (_velocity.X < 0)
				flip = SpriteEffects.FlipHorizontally;
			else if (_velocity.X > 0)
				flip = SpriteEffects.None;

			Vector2 actualPos = new Vector2((_position.X - cameraOffsetX) * scaleVector.X, (_position.Y + _position.Z) * scaleVector.Y);
			float scale = scaleVector.Y * (_position.Y / GameLogic.BaseViewPort.Height);

			if (GameLogic.DebugDraw)
			{
				spriteBatch.Draw(
				GameLogic.DefaultRectangle,
				new Vector2((swordCollider.Min.X - cameraOffsetX) * scaleVector.X, (swordCollider.Min.Y + swordCollider.Min.Z) * scaleVector.Y),
				new Rectangle(0, 0, (int)((swordCollider.Max.X - swordCollider.Min.X) * scale), (int)((swordCollider.Max.Y - swordCollider.Min.Y) * scale)),
				Color.Chocolate);
			}

			animationObj.Draw(gameTime, spriteBatch, actualPos, flip, scale);
		}

		public override void TakeDamage(int damage)
		{
			base.TakeDamage(damage);

			if (isDefending)
			{
				_hp += damage;
			}
			else
				isHurt = true;

			if (!IsAlive)
			{
				animationObj.PlayAnimation(animationDeath);
			}
		}
	}
}
