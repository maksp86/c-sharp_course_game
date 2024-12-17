using JABEUP_Game.Game.Controller;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JABEUP_Game
{
	public interface IDrawableGameEntity
	{
		public void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController);
		public void LoadContent(ContentManager contentManager) { }
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX);
	}
}
