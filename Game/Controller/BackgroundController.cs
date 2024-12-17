using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace JABEUP_Game.Game.Controller
{
	public class Layer
	{
		private readonly Texture2D _texture;
		private Vector2 _position = Vector2.Zero;
		private Vector2 _position2 = Vector2.Zero;
		private readonly float _depth, _moveScale, _defaultSpeed;
		private Vector2 backgroundScaleVector = Vector2.One;

		public Layer(Texture2D texture, float depth, float moveScale, float defaultSpeed = 0.0f)
		{
			_depth = depth;
			_moveScale = moveScale;
			_defaultSpeed = defaultSpeed;
			_texture = texture;
			backgroundScaleVector = new Vector2((float)GameLogic.BaseViewPort.Width / _texture.Width, (float)GameLogic.BaseViewPort.Height / _texture.Height);
		}

		public void Initialize()
		{
			_position.X = 0;
		}


		public void Update(float movement, GameTime gameTime)
		{
			_position.X += (movement * _moveScale / backgroundScaleVector.X + _defaultSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			_position.X %= _texture.Width;

			if (_position.X >= 0)
			{
				_position2.X = _position.X - _texture.Width;
			}
			else
			{
				_position2.X = _position.X + _texture.Width;
			}
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 _scaleVector)
		{
			spriteBatch.Draw(_texture, _position * backgroundScaleVector * _scaleVector, null, Color.White, 0, Vector2.Zero, backgroundScaleVector * _scaleVector, SpriteEffects.None, _depth);
			spriteBatch.Draw(_texture, _position2 * backgroundScaleVector * _scaleVector, null, Color.White, 0, Vector2.Zero, backgroundScaleVector * _scaleVector, SpriteEffects.None, _depth);
		}
	}

	public class BackgroundController
	{
		private List<Layer> _layers;

		public BackgroundController()
		{
			_layers = new();
		}

		public void Initialize()
		{
			foreach (var layer in _layers)
			{
				layer.Initialize();
			}
		}

		public void AddLayer(Layer layer)
		{
			_layers.Add(layer);
		}

		public void Update(float movement, GameTime gameTime)
		{
			foreach (var layer in _layers)
			{
				layer.Update(movement, gameTime);
			}
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 scaleVector)
		{
			foreach (var layer in _layers)
			{
				layer.Draw(spriteBatch, scaleVector);
			}
		}
	}
}
