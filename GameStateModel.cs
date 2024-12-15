using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JABEUP_Game
{
	public class GameStateChangedEventArgs : EventArgs
	{
		public GameState oldValue, newValue;
		public GameStateChangedEventArgs(GameState oldValue, GameState newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}

	public enum GameState
	{
		Menu,
		PauseMenu,
		Game
	}

	public class GameStateModel
	{
		public event EventHandler<GameStateChangedEventArgs> StateChanged;

		public GameState GameState => _gameState;
		private GameState _gameState = GameState.Menu;

		public void SetGameState(object sender, GameState gameState)
		{
			StateChanged?.Invoke(sender, new GameStateChangedEventArgs(_gameState, gameState));
			_gameState = gameState;
		}


	}
}
