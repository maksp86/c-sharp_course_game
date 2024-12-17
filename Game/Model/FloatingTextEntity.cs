using JABEUP_Game.Game.Controller;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace JABEUP_Game.Game.Model
{
	public class FloatingTextEntity : ICollaidableGameEntity
	{
		public BoundingBox Collider => _collider;
		private BoundingBox _collider;

		public Vector3 Position => _position;
		private Vector3 _position;

		public Vector3 Velocity => throw new NotImplementedException();

		public ColliderType ColliderType => throw new NotImplementedException();

		SpriteFont _font;
		string _text;
		Vector2 _textOrigin;
		float _scale;

		public FloatingTextEntity(SpriteFont font, string text, float scale = 1f)
		{
			_font = font;
			_text = text;
			_scale = scale;
		}

		public void Initialize(Vector2 spawnPosition)
		{
			Reset(spawnPosition);
			_textOrigin = _font.MeasureString(_text) / 2;
			_collider = new BoundingBox(_position - new Vector3(_textOrigin, 0), _position + new Vector3(_textOrigin, 0));
		}

		public void Reset(Vector2 withPosition)
		{
			_position = new Vector3(withPosition.X, withPosition.Y, 0);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
		{
			Vector2 actualPos = new Vector2((_position.X - cameraOffsetX) * scaleVector.X, (_position.Y + _position.Z) * scaleVector.Y);

			spriteBatch.DrawString(_font, _text, actualPos, Color.Black, 0f, _textOrigin, scaleVector.X * _scale * 1.02f, SpriteEffects.None, 1f);
			spriteBatch.DrawString(_font, _text, actualPos, Color.White, 0f, _textOrigin, scaleVector.X * _scale, SpriteEffects.None, 1f);
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController) { }

		public void UpdateCollisions(IEnumerable<ICollaidableGameEntity> collaidables, GameTime gameTime) { }
	}
}
