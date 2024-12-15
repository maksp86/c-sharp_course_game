using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JABEUP_Game.Game.View
{
	public class ProgressBarRenderer : IDrawableGameEntity
	{
		private protected Texture2D background;
		private protected Texture2D foreground;
		private protected Vector3 position;
		private protected float maxValue;
		private protected float currentValue;
		private protected Rectangle innerPart;
		private protected float scale = 1f;

		public ProgressBarRenderer()
		{
		}

		public virtual void LoadContent(ContentManager contentManager)
		{
			background = contentManager.Load<Texture2D>("GUI/HealthBar/Empty");
			foreground = contentManager.Load<Texture2D>("GUI/HealthBar/Filler");
			innerPart = new(0, 0, foreground.Width, foreground.Height);
		}

		public virtual void UpdateValue(float value, float max, Vector3 drawPosition)
		{
			currentValue = value;
			maxValue = max;
			innerPart.Width = (int)(currentValue / maxValue * foreground.Width);
			position = drawPosition;
		}

		public void SetScale(float scale)
		{
			this.scale = scale;
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
		{
			Vector2 actualPos = new Vector2((position.X - cameraOffsetX) * scaleVector.X, (position.Y + position.Z) * scaleVector.Y);

			spriteBatch.Draw(background, actualPos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
			spriteBatch.Draw(foreground, actualPos, innerPart, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
		}

		public virtual void Update(GameTime gameTime, KeyboardState keyboardState) { }

	}

	//public class ProgressBarAnimatedRenderer : ProgressBarRenderer
	//{
	//	private float _targetValue;
	//	private readonly float _animationSpeed;
	//	private Rectangle _animationPart;
	//	private Vector3 _animationPosition;
	//	private Color _animationShade;
	//	private float value;

	//	public ProgressBarAnimatedRenderer(float max) : base(max)
	//	{
	//		_targetValue = max;
	//		_animationSpeed = max / 10;
	//		_animationShade = Color.DarkGray;
	//	}

	//	public override void LoadContent(ContentManager contentManager)
	//	{
	//		base.LoadContent(contentManager);
	//		_animationPart = new(foreground.Width, 0, 0, foreground.Height);
	//	}

	//	public override void UpdateValue(float value, Vector3 drawPosition)
	//	{
	//		this.value = value;
	//		_animationPosition = drawPosition;
	//		innerPart.Width = (int)(currentValue / maxValue * foreground.Width);
	//		position = drawPosition;
	//	}

	//	public override void Update(GameTime gameTime, KeyboardState keyboardState)
	//	{
	//		if (value == currentValue) return;

	//		_targetValue = value;
	//		int x;
	//		float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
	//		if (_targetValue < currentValue)
	//		{
	//			currentValue -= _animationSpeed * elapsedSeconds;
	//			if (currentValue < _targetValue) currentValue = _targetValue;
	//			x = (int)(_targetValue / maxValue * foreground.Width);
	//			_animationShade = Color.Gray;
	//		}
	//		else
	//		{
	//			currentValue += _animationSpeed * elapsedSeconds;
	//			if (currentValue > _targetValue) currentValue = _targetValue;
	//			x = (int)(currentValue / maxValue * foreground.Width);
	//			_animationShade = Color.DarkGray * 0.5f;
	//		}

	//		innerPart.Width = x;
	//		_animationPart.X = x;
	//		_animationPart.Width = (int)(Math.Abs(currentValue - _targetValue) / maxValue * foreground.Width);
	//		_animationPosition.X = position.X + x;
	//	}

	//	public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
	//	{
	//		base.Draw(gameTime, spriteBatch, scaleVector, cameraOffsetX);

	//		Vector2 actualPos = new Vector2((position.X - cameraOffsetX) * scaleVector.X, (position.Y + position.Z) * scaleVector.Y);
	//		spriteBatch.Draw(foreground, actualPos, _animationPart, _animationShade, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
	//	}
	//}
}
