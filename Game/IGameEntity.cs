using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace JABEUP_Game.Game
{
	public interface IGameEntity
	{
		public void Update(GameTime gameTime, KeyboardState keyboardState);
		public void LoadContent(ContentManager contentManager) { }
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
	}
}
