using JABEUP_Game.Game.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JABEUP_Game.Game.Controller
{
	public class EnvironmentSafeZoneController
	{
		private List<SafeZonePolygon> _safeZonePolygons = new List<SafeZonePolygon>();

		List<Vector2> points = new List<Vector2>() {
				new Vector2(0,720),
				new Vector2(0,531),
				new Vector2(206,480),
				new Vector2(304,490),
				new Vector2(500,550),
				new Vector2(570,590),
				new Vector2(660,610),
				new Vector2(700,630),
				new Vector2(883,660),
				new Vector2(1060,560),
				new Vector2(1100,550),
				new Vector2(1100,536),
				new Vector2(1235,525),
				new Vector2(1280,530),
				new Vector2(1280,720),
			};

		public EnvironmentSafeZoneController()
		{

		}

		private long lastUpdatedOffset = 0;

		public void Update(float cameraOffsetX)
		{
			if (cameraOffsetX >= (_safeZonePolygons[1].OffsetX) && (long)cameraOffsetX != lastUpdatedOffset)
			{
				lastUpdatedOffset = (long)cameraOffsetX;
				_safeZonePolygons.RemoveAt(0);
				_safeZonePolygons.Add(new SafeZonePolygon(points, _safeZonePolygons[0].OffsetX + GameLogic.BaseViewPort.Width));

			}
		}

		public bool IsInSafeZone(Vector3 position)
		{
			if (position.Y > 660 && position.Y < GameLogic.BaseViewPort.Height)
				return true;

			for (int i = 0; i < _safeZonePolygons.Count; i++)
			{
				bool result = _safeZonePolygons[i].IsPointInPolygon(new Vector2(position.X, position.Y));
				if (result)
					return true;
			}
			return false;
		}

		public void DrawLineBetween(SpriteBatch spriteBatch, Vector2 startPos, Vector2 endPos, int thickness, Color color)
		{
			// Create a texture as wide as the distance between two points and as high as
			// the desired thickness of the line.
			var distance = (int)Vector2.Distance(startPos, endPos);
			var texture = new Texture2D(spriteBatch.GraphicsDevice, distance, thickness);

			// Fill texture with given color.
			var data = new Color[distance * thickness];
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = color;
			}
			texture.SetData(data);

			// Rotate about the beginning middle of the line.
			var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
			var origin = new Vector2(0, thickness / 2);

			spriteBatch.Draw(
				texture,
				startPos,
				null,
				Color.White,
				rotation,
				origin,
				1.0f,
				SpriteEffects.None,
				1.0f);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
		{
			if (GameLogic.DebugDraw)
			{
				foreach (SafeZonePolygon zonePolygon in _safeZonePolygons)
				{
					foreach ((Vector2 point1, Vector2 point2) in points.Zip(points.Skip(1), (a, b) => Tuple.Create(a, b)))
					{
						DrawLineBetween(spriteBatch,
							new Vector2(point1.X + zonePolygon.OffsetX - cameraOffsetX, point1.Y) * scaleVector,
							new Vector2(point2.X + zonePolygon.OffsetX - cameraOffsetX, point2.Y) * scaleVector,
							(int)Math.Ceiling(2 * scaleVector.X),
							Color.Red);
					}
				}
			}
		}

		public void Initialize()
		{
			_safeZonePolygons.Clear();
			_safeZonePolygons.Add(new SafeZonePolygon(points, 0));
			_safeZonePolygons.Add(new SafeZonePolygon(points, GameLogic.BaseViewPort.Width));
		}
	}
}
