using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JABEUP_Game.Game.Player
{
	class PlayerClass : IGameEntity
	{
		private Animation animationIdle, animationRun, animationRunAttack, animationJump, animationAttack, animationAttackDefend, animationDefend;
		private AnimationPlayer animationObj;
		private SpriteEffects flip = SpriteEffects.None;

		public bool IsAlive { get; private set; }
		public bool IsOnGround { get; private set; }
		public Vector3 Velocity => velocity;

		private Vector3 position;
		private Vector3 velocity;

		private Vector2 movement;

		private bool isDefending;
		private bool isAttacking;
		private TimeSpan attackEndTime;
		private Animation attackAnimation;

		private bool isJumping;
		private bool wasJumping;
		private float jumpTime;

		// Constants for controlling horizontal movement
		private const float MoveAcceleration = 13000.0f;
		private const float MaxMoveSpeed = 1750.0f;
		private const float GroundDragFactor = 0.48f;
		private const float AirDragFactor = 0.58f;

		// Constants for controlling vertical movement
		private const float MaxJumpTime = 0.35f;
		private const float JumpLaunchVelocity = -3500.0f;
		private const float GravityAcceleration = 3400.0f;
		private const float MaxFallSpeed = 550.0f;
		private const float JumpControlPower = 0.14f;

		public PlayerClass()
		{
			velocity = Vector3.Zero;
			position = Vector3.Zero;
			movement = Vector2.Zero;

		}

		public void Initialize(Vector2 spawnPosition)
		{
			Reset(spawnPosition);
		}

		public void LoadContent(ContentManager content)
		{
			animationIdle = new Animation(content.Load<Texture2D>("Knight_1/Idle"), 0.2f, true);
			animationRun = new Animation(content.Load<Texture2D>("Knight_1/Run"), 0.2f, true);
			animationJump = new Animation(content.Load<Texture2D>("Knight_1/Jump"), 0.2f, false);
			animationRunAttack = new Animation(content.Load<Texture2D>("Knight_1/Run+Attack"), 0.2f, false);
			animationAttack = new Animation(content.Load<Texture2D>("Knight_1/Attack 1"), 0.2f, false);
			animationAttackDefend = new Animation(content.Load<Texture2D>("Knight_1/Attack 3"), 0.2f, false);
			animationDefend = new Animation(content.Load<Texture2D>("Knight_1/Defend"), 0.2f, true);
		}

		public void Reset(Vector2 withPosition)
		{
			isAttacking = false;
			isDefending = false;
			IsAlive = true;
			animationObj.PlayAnimation(animationIdle);
			position = new Vector3(withPosition.X, withPosition.Y, 0);
			movement = Vector2.Zero;
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
			_handleActions(keyboardState, gameTime);
			_handleMovement(keyboardState);

			DoPhysics(gameTime);

			if (isAttacking)
			{
				if (gameTime.TotalGameTime >= attackEndTime)
					isAttacking = false;
				animationObj.PlayAnimation(attackAnimation);
			}
			else if (IsAlive && IsOnGround)
			{
				if (isDefending)
					animationObj.PlayAnimation(animationDefend);
				else if (velocity.X == 0 && movement.X == 0 && movement.Y == 0)
					animationObj.PlayAnimation(animationIdle);
				else
					animationObj.PlayAnimation(animationRun);
			}

			movement = Vector2.Zero;
			isJumping = false;
		}

		private void _handleActions(KeyboardState keyboardState, GameTime gameTime)
		{
			if (keyboardState.IsKeyDown(Keys.LeftControl) && !isAttacking && gameTime.TotalGameTime - attackEndTime > TimeSpan.FromMilliseconds(500))
			{
				isAttacking = true;
				attackEndTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(1);
				if (velocity.X != 0 || velocity.Y != 0)
					attackAnimation = animationRunAttack;
				else if (isDefending)
					attackAnimation = animationAttackDefend;
				else
				{
					velocity.X += 50;
					attackAnimation = animationAttack; 
				}
			}

			isDefending = !isJumping && keyboardState.IsKeyDown(Keys.LeftShift);
		}

		private void _handleMovement(KeyboardState keyboardState)
		{
			if (isDefending)
				return;

			if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
				movement.X = -1.0f;
			else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
				movement.X = 1.0f;

			if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
				movement.Y = 1.0f;
			else if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
				movement.Y = -1.0f;

			isJumping = keyboardState.IsKeyDown(Keys.Space);
		}

		public void DoPhysics(GameTime gameTime)
		{
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Vector3 previousPosition = position;

			velocity.X += movement.X * MoveAcceleration * elapsed;

			velocity.Y += movement.Y * MoveAcceleration / 16 * 9 * elapsed;

			velocity.Z = MathHelper.Clamp(velocity.Z + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

			velocity.Z = DoJump(velocity.Z, gameTime);

			if (IsOnGround)
			{
				velocity.X *= GroundDragFactor;
				velocity.Y *= GroundDragFactor;
			}
			else
			{
				velocity.X *= AirDragFactor;
				velocity.Y *= AirDragFactor;
			}

			velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
			velocity.Y = MathHelper.Clamp(velocity.Y, -MaxMoveSpeed / 16 * 9, MaxMoveSpeed / 16 * 9);

			position += velocity * elapsed;
			position = new Vector3((float)Math.Round(position.X), (float)Math.Round(position.Y), (float)Math.Round(position.Z));

			HandleCollisions();

			if (position.X == previousPosition.X)
				velocity.X = 0;

			if (position.Y == previousPosition.Y)
				velocity.Y = 0;

			if (position.Z == previousPosition.Z)
				velocity.Z = 0;
		}

		private float DoJump(float velocityY, GameTime gameTime)
		{
			// If the player wants to jump
			if (isJumping)
			{
				// Begin or continue a jump
				if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
				{
					jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
					animationObj.PlayAnimation(animationJump);
				}

				// If we are in the ascent of the jump
				if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
				{
					// Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
					velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
				}
				else
					jumpTime = 0.0f;
			}
			else
				jumpTime = 0.0f;
			wasJumping = isJumping;

			return velocityY;
		}

		private void HandleCollisions()
		{
			// Reset flag to search for ground collision.
			IsOnGround = false;

			if (position.X < 0)
				position.X = 0;
			else if (position.X > 1280)
				position.X = 1280;

			if (position.Y > 720)
				position.Y = 720;
			else if (position.Y < 420)
				position.Y = 420;

			if (position.Z > 0)
				position.Z = 0;

			if (position.Z == 0)
				IsOnGround = true;

		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (velocity.X < 0)
				flip = SpriteEffects.FlipHorizontally;
			else if (velocity.X > 0)
				flip = SpriteEffects.None;

			animationObj.Draw(gameTime, spriteBatch, new Vector2(position.X, position.Y + position.Z), flip, (position.Y / 720));
		}
	}
}
