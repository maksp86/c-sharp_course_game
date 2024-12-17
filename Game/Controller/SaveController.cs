using JABEUP_Game.Game.Model;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JABEUP_Game.Game.Controller
{
	public class SaveController
	{
		private string _saveFileName;

		public Save CurrentData => _currentData;
		private Save _currentData;


		public SaveController(string saveFileName = "game.save")
		{
			_saveFileName = saveFileName;
			_currentData = new Save();
		}

		public void Save()
		{
			_currentData.SaveTime = DateTime.Now;
			File.WriteAllText(_saveFileName, JsonConvert.SerializeObject(_currentData));
		}

		public void Load()
		{
			if (File.Exists(_saveFileName))
				_currentData = JsonConvert.DeserializeObject<Save>(File.ReadAllText(_saveFileName));
			else _currentData = new Save()
			{
				Options = new Options()
				{
					MusicVolume = 1f,
					SoundVolume = 1f,
					KeyBindings = new Dictionary<string, Keys>() {
						{ "Up", Keys.W },
						{ "Down", Keys.S },
						{ "Left", Keys.A },
						{ "Right", Keys.D },
						{ "Jump", Keys.Space },
						{ "Defend", Keys.LeftShift },
						{ "Attack", Keys.LeftControl },
					}
				},
				HighScore = 0
			};
		}
	}
}
