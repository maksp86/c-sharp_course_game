using JABEUP_Game.Game.Controller;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

			spriteBatch.Draw(background, actualPos, null, Color.White, 0, Vector2.Zero, scale * scaleVector.X, SpriteEffects.None, 1f);
			spriteBatch.Draw(foreground, actualPos, innerPart, Color.White, 0, Vector2.Zero, scale * scaleVector.X, SpriteEffects.None, 1f);
		}

		public virtual void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController) { }

	}
}
