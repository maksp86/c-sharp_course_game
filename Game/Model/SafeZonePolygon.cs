using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace JABEUP_Game.Game.Model
{
	public class SafeZonePolygon
	{
		public float OffsetX => _offsetX;

		private List<Vector2> _points;
		private float _offsetX = 0;

		public SafeZonePolygon(List<Vector2> points, float offsetX)
		{
			_points = points;
			_offsetX = offsetX;
		}

		public bool IsPointInPolygon(Vector2 testPoint)
		{
			bool result = false;
			int j = _points.Count - 1;
			for (int i = 0; i < _points.Count; i++)
			{
				if (_points[i].Y < testPoint.Y && _points[j].Y >= testPoint.Y ||
					_points[j].Y < testPoint.Y && _points[i].Y >= testPoint.Y)
				{
					if ((_points[i].X + _offsetX) + (testPoint.Y - _points[i].Y) /
					   (_points[j].Y - _points[i].Y) *
					   ((_points[j].X + _offsetX) - (_points[i].X + _offsetX)) < testPoint.X)
					{
						result = !result;
					}
				}
				j = i;
			}
			return result;
		}
	}
}
