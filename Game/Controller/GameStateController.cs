using JABEUP_Game.Game.Model;
using System;

namespace JABEUP_Game.Game.Controller
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
		DeadMenu,
		Game
	}

	public class GameStateController
	{
		public event EventHandler<GameStateChangedEventArgs> StateChanged;


		public GameState GameState => _gameState;
#if DEBUG
		private GameState _gameState = GameState.Menu;
#else
		private GameState _gameState = GameState.Menu;
#endif

		public long Score => _score + _enemyScore;
		private long _score, _enemyScore;

		public void UpdateScore(float cameraOffsetX)
		{
			_score = (long)Math.Ceiling(cameraOffsetX / (GameLogic.BaseViewPort.Width / 5f));
		}

		public void AddEnemyScore(AliveGameEntity killedEntity)
		{
			_enemyScore += killedEntity.MaxHP / 5;
		}

		public void ClearScore()
		{
			_score = 0;
			_enemyScore = 0;
		}

		public void SetGameState(object sender, GameState gameState)
		{
			StateChanged?.Invoke(sender, new GameStateChangedEventArgs(_gameState, gameState));
			_gameState = gameState;
		}


	}
}
