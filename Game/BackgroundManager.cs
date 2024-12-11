using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JABEUP_Game.Game
{
	public class Layer
	{
		private readonly Texture2D _texture;
		private Vector2 _position = Vector2.Zero;
		private Vector2 _position2 = Vector2.Zero;
		private readonly float _depth, _moveScale, _defaultSpeed;
		private Vector2 scaleVector = Vector2.One;

		public Layer(Texture2D texture, float depth, float moveScale, float defaultSpeed = 0.0f)
		{
			_depth = depth;
			_moveScale = moveScale;
			_defaultSpeed = defaultSpeed;
			_texture = texture;
		}

		public void Update(float movement, GameTime gameTime)
		{
			_position.X += ((movement * _moveScale) + _defaultSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			_position.X %= _texture.Width * scaleVector.X;

			if (_position.X >= 0)
			{
				_position2.X = _position.X - _texture.Width * scaleVector.X;
			}
			else
			{
				_position2.X = _position.X + _texture.Width * scaleVector.X;
			}
		}

		public void Draw(SpriteBatch spriteBatch, Rectangle viewPort)
		{
			scaleVector = new Vector2((float)viewPort.Width / _texture.Width, (float)viewPort.Height / _texture.Height);
			spriteBatch.Draw(_texture, _position, null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, _depth);
			spriteBatch.Draw(_texture, _position2, null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, _depth);
		}
	}

	public class BackgroundManager
	{
		private readonly List<Layer> _layers;

		public BackgroundManager()
		{
			_layers = new();
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

		public void Draw(SpriteBatch spriteBatch, Rectangle viewPort)
		{
			foreach (var layer in _layers)
			{
				layer.Draw(spriteBatch, viewPort);
			}
		}
	}
}
