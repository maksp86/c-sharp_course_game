using JABEUP_Game.Game.Controller;
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

namespace JABEUP_Game.Game.Enemy
{
	public class EnemySlimeClass : AliveGameEntity
	{
		private Animation animationIdle, animationRun, animationAttack, animationDeath, animationHurt;
		private AnimationController animationObj;
		private SpriteEffects flip = SpriteEffects.None;

		public override AliveEntityType EntityType => AliveEntityType.Mob;
		public override ColliderType ColliderType => ColliderType.Strong;
		public override int MaxHP => 60;

		private BoundingBox target;
		private BoundingBox attackCollider;

		private bool isHurt;
		private TimeSpan hurtEndTime;

		private bool isAttacking;
		private bool attackHappened;
		private TimeSpan attackEndTime;

		private ProgressBarRenderer _healthBar;

		private readonly string[] slimeColors = new[] { "Red", "Green", "Blue" };
		private string _color;

		private const float _slimeScale = 1f;

		public EnemySlimeClass()
		{
			_velocity = Vector3.Zero;
			_position = Vector3.Zero;
			_movement = Vector2.Zero;
			_color = slimeColors[Random.Shared.Next(slimeColors.Length)];
			_healthBar = new ProgressBarRenderer();
		}

		public override void LoadContent(ContentManager content)
		{
			animationIdle = new Animation(content.Load<Texture2D>($"Sprites/Slime/{_color}/Idle"), 0.2f, true, true);
			animationRun = new Animation(content.Load<Texture2D>($"Sprites/Slime/{_color}/Walk"), 0.2f, true, true);
			animationAttack = new Animation(content.Load<Texture2D>($"Sprites/Slime/{_color}/Attack"), 0.15f, false, true);
			animationDeath = new Animation(content.Load<Texture2D>($"Sprites/Slime/{_color}/Dead"), 0.15f, false, true);
			animationHurt = new Animation(content.Load<Texture2D>($"Sprites/Slime/{_color}/Hurt"), 0.1f, false, true);
			_healthBar.LoadContent(content);
		}

		public override void Initialize(Vector2 withPosition)
		{
			base.Initialize(withPosition);
			isAttacking = false;
			isHurt = false;
			animationObj.PlayAnimation(animationIdle);
			_collider = new BoundingBox(_position, _position);
		}

		public void SetTarget(BoundingBox targetCollider)
		{
			target = targetCollider;
		}

		public override void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController)
		{
			if (!IsAlive)
				return;

			if (Vector3.Distance(_position, target.Max) > (animationIdle.FrameHeight * 6))
			{
				int nextMove = Random.Shared.Next(-10, 10);
				if (nextMove == 1)
					_movement.X = (float)Random.Shared.Next(-5, 5);
				else if (nextMove == 0)
					_movement.Y = (float)Random.Shared.Next(-5, 5);
			}
			else
			{
				if ((!Collider.Intersects(target) || Math.Abs(_position.Y - target.Max.Y) > animationIdle.FrameHeight / 2)
					&& Math.Min(Vector3.Distance(_collider.Max, target.Min), Vector3.Distance(_collider.Max, target.Min)) > animationIdle.FrameHeight)
				{
					if (!isAttacking)
					{
						if (_position.X > target.Min.X)
							_movement.X = -1 * (float)Random.Shared.NextDouble();
						else if (_position.X < target.Min.X)
							_movement.X = 1 * (float)Random.Shared.NextDouble();

						if (_position.Y > target.Max.Y)
							_movement.Y = -1 * (float)Random.Shared.NextDouble();
						else if (_position.Y < target.Max.Y)
							_movement.Y = 1 * (float)Random.Shared.NextDouble();
					}
				}
				else if (gameTime.TotalGameTime - attackEndTime > TimeSpan.FromMilliseconds(1000) && !isHurt)
				{
					isAttacking = true;
					attackHappened = false;
					attackEndTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(animationAttack.FrameTime * animationAttack.FrameCount);
				}
			}

			DoPhysics(gameTime, safeZoneController);

			_collider.Min = new Vector3(animationIdle.FrameWidth * 0.6f + _position.X, _position.Y - (animationIdle.FrameHeight / 1.1f), _position.Z);
			_collider.Max = _collider.Min + new Vector3(animationIdle.FrameWidth * 0.8f, animationIdle.FrameHeight, 0);

			if (isHurt)
			{
				animationObj.PlayAnimation(animationHurt);

				if (hurtEndTime == TimeSpan.Zero)
					hurtEndTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(animationHurt.FrameTime * animationHurt.FrameCount);

				if (gameTime.TotalGameTime >= hurtEndTime)
				{
					hurtEndTime = TimeSpan.Zero;
					isHurt = false;
				}
			}
			else if (isAttacking)
				animationObj.PlayAnimation(animationAttack);
			else if (IsAlive)
			{
				if (_velocity.X == 0 && _movement.X == 0 && _movement.Y == 0)
					animationObj.PlayAnimation(animationIdle);
				else
					animationObj.PlayAnimation(animationRun);
			}


			_healthBar.UpdateValue(HP, MaxHP, new Vector3(_position.X + (animationIdle.FrameHeight) / 2, _position.Y - (animationIdle.FrameHeight), _position.Z));
			_healthBar.Update(gameTime, keyboardState, safeZoneController);
			_movement = Vector2.Zero;
		}

		public override void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime)
		{
			PlayerClass player = collaidables
				.Where(c => c is PlayerClass)
				.Select(c => c as PlayerClass)
				.First();
			//.Select(c => c as PlayerClass)
			if (player.IsAlive)
				target = player.Collider;
			else
				target = new BoundingBox(Vector3.Zero, Vector3.Zero);

			if (isAttacking)
			{
				if (!attackHappened && animationObj.FrameIndex >= (animationObj.Animation.FrameCount / 2))
				{
					attackHappened = true;
					if (flip == SpriteEffects.FlipHorizontally)
						attackCollider = new BoundingBox(new Vector3(_collider.Min.X - animationIdle.FrameHeight / 3, _collider.Min.Y, _collider.Min.Z),
							new Vector3(_collider.Min.X, _collider.Max.Y, _collider.Max.Z));
					else
						attackCollider = new BoundingBox(new Vector3(_collider.Max.X, _collider.Min.Y, _collider.Min.Z),
							new Vector3(_collider.Max.X + animationIdle.FrameHeight / 3, _collider.Max.Y, _collider.Max.Z));

					if (player.Collider.Intersects(attackCollider))
						player.TakeDamage(50);
				}
				if (gameTime.TotalGameTime >= attackEndTime)
				{
					isAttacking = false;
					attackHappened = false;
				}
				animationObj.PlayAnimation(animationAttack);
			}
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
					new Vector2((attackCollider.Min.X - cameraOffsetX) * scaleVector.X, (attackCollider.Min.Y + attackCollider.Min.Z) * scaleVector.Y),
					new Rectangle(0, 0, (int)((attackCollider.Max.X - attackCollider.Min.X) * scale), (int)((attackCollider.Max.Y - attackCollider.Min.Y) * scale)),
					Color.Chocolate);
			}

			animationObj.Draw(gameTime, spriteBatch, actualPos, flip, scale);

			if (IsAlive)
			{
				_healthBar.SetScale(scale * 0.8f);
				_healthBar.Draw(gameTime, spriteBatch, scaleVector, cameraOffsetX);
			}

		}

		public override void TakeDamage(int damage)
		{
			base.TakeDamage(damage);

			isHurt = true;

			if (!IsAlive)
			{
				isAttacking = false;
				animationObj.PlayAnimation(animationDeath);
			}
		}
	}
}
