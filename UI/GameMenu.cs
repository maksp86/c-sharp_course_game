using AssetManagementBase;
using JABEUP_Game.Game.Controller;
using JABEUP_Game.UI.MyraDeathScreen;
using JABEUP_Game.UI.MyraMenu;
using JABEUP_Game.UI.MyraOptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Reflection;


namespace JABEUP_Game.UI
{
	public class GameMenu : IDrawableGameEntity
	{
		private string[] randomTexts = new string[] { "Now with sound!", "Better than Minecraft!", "As seen on TikTok!", "Family frienly!", "Beat`em up!", "Also try NFT!", "Also try Terraria!" };

		private MenuLayout _menuLayout;
		private OptionsLayout _optionsLayout;
		private DeathScreenLayout _deathScreenLayout;
		private Desktop _desktop;

		TimeSpan _hideTextTime = TimeSpan.Zero;
		GameStateController _gameStateModel;
		SaveController _saveEngine;

		bool animationDirection = false;
		float animationValue = 0f;
		const float animationSpeed = 0.02f;

		public GameMenu(GameStateController gameStateModel, SaveController saveEngine)
		{
			_gameStateModel = gameStateModel;
			_saveEngine = saveEngine;
		}

		private void OnGameStateChanged(object sender, GameStateChangedEventArgs e)
		{
			Label playButtonLabel = _menuLayout.playButton.FindChild<Label>();
			Label exitButtonLabel = _menuLayout.exitButton.FindChild<Label>();

			switch (e.newValue)
			{
				case GameState.PauseMenu:
					if (_saveEngine.CurrentData.HighScore > 0)
					{
						_menuLayout.hiscoreText.Visible = true;
						_menuLayout.hiscoreText.Text = "High score: " + _saveEngine.CurrentData.HighScore;
					}
					playButtonLabel.Text = "Resume";
					exitButtonLabel.Text = "Go to menu";
					_menuLayout.pausedText.Visible = true;
					_menuLayout.randomText.Visible = false;
					break;
				case GameState.DeadMenu:
					_deathScreenLayout.scoreText.Text = "Your score: " + _gameStateModel.Score;
					_desktop.Root = _deathScreenLayout;
					break;

				case GameState.Menu:
					playButtonLabel.Text = "Play";
					exitButtonLabel.Text = "Exit";
					_menuLayout.randomText.Visible = true;
					_menuLayout.randomText.Text = randomTexts[Random.Shared.Next(randomTexts.Length)];
					_menuLayout.pausedText.Visible = false;
					break;

				default:
					playButtonLabel.Text = "Play";
					exitButtonLabel.Text = "Exit";
					_menuLayout.randomText.Visible = false;
					_menuLayout.pausedText.Visible = false;
					break;
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 scaleVector, float cameraOffsetX)
		{
			if (_gameStateModel.GameState == GameState.Menu)
			{
				Vector2 randomTextPos = new Vector2(GameLogic.BaseViewPort.Width * 0.65f, GameLogic.BaseViewPort.Height * 0.3f) * scaleVector;


				if (animationDirection)
				{
					if (animationValue < 1f)
						animationValue += animationSpeed;
					else animationDirection = false;
				}
				else
				{
					if (animationValue > 0f)
						animationValue -= animationSpeed;
					else animationDirection = true;
				}
				animationValue = Math.Clamp(animationValue, 0f, 1f);
				
				Vector2 randomTextScale = new Vector2(0.8f, 0.8f) + (new Vector2(0.25f, 0.25f) * animationValue);
				if (scaleVector.X < 1)
					randomTextScale *= scaleVector;

				_menuLayout.randomText.Top = (int)randomTextPos.Y;
				_menuLayout.randomText.Left = (int)randomTextPos.X;
				_menuLayout.randomText.Scale = randomTextScale;
				_menuLayout.randomText.Rotation = 15 + animationValue * 15;

			}

			_desktop.Render();
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState, EnvironmentSafeZoneController safeZoneController)
		{
			if (_desktop.Root == _optionsLayout && _optionsLayout.textSaved.Visible)
			{
				if (_hideTextTime == TimeSpan.Zero)
					_hideTextTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(2));

				if (gameTime.TotalGameTime >= _hideTextTime)
				{
					_optionsLayout.textSaved.Visible = false;
					_hideTextTime = TimeSpan.Zero;
				}
			}

			if (_gameStateModel.GameState == GameState.PauseMenu
				&& _desktop.Root == _menuLayout
				&& gameTime.TotalGameTime >= _hideTextTime)
			{
				_menuLayout.pausedText.Visible = !_menuLayout.pausedText.Visible;
				_hideTextTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(0.5f));
			}
		}

		public string currentEditingKeybind;

		public void LoadContent(ContentManager contentManager)
		{
			AssetManager assetManager = AssetManager.CreateResourceAssetManager(typeof(GameLogic).Assembly, "UI.Resources");

			// Load stylesheet
			Stylesheet.Current = assetManager.LoadStylesheet("ui_stylesheet.xmms");

			_menuLayout = new MenuLayout();
			_optionsLayout = new OptionsLayout();
			_deathScreenLayout = new DeathScreenLayout();

			var textureAtlas = assetManager.LoadTextureRegionAtlas("ui_stylesheet.xmat");

			_desktop = new Desktop
			{
				Root = _menuLayout
			};

			_deathScreenLayout.respawnButton.Click += (s, e) => { _gameStateModel.SetGameState(this, GameState.Game); };
			_deathScreenLayout.menuButton.Click += (s, e) => { _desktop.Root = _menuLayout; };

			_menuLayout.playButton.Click += (s, e) => { _gameStateModel.SetGameState(this, GameState.Game); };
			_menuLayout.exitButton.Click += (s, e) =>
			{
				if (_gameStateModel.GameState == GameState.PauseMenu)
					_gameStateModel.SetGameState(this, GameState.Menu);
				else if (_gameStateModel.GameState == GameState.Menu)
					MyraEnvironment.Game.Exit();
			};
			_menuLayout.optionsButton.Click += (s, e) => { _desktop.Root = _optionsLayout; };

			_optionsLayout.musicVolumeSlider.Value = _saveEngine.CurrentData.Options.MusicVolume;
			_optionsLayout.soundVolumeSlider.Value = _saveEngine.CurrentData.Options.SoundVolume;
			_optionsLayout.backButton.Click += (s, e) => { _desktop.Root = _menuLayout; };
			_optionsLayout.saveButton.Click += (s, e) =>
			{
				_saveEngine.CurrentData.Options.MusicVolume = _optionsLayout.musicVolumeSlider.Value;
				_saveEngine.CurrentData.Options.SoundVolume = _optionsLayout.soundVolumeSlider.Value;
				_optionsLayout.textSaved.Visible = true;
			};

			if (_saveEngine.CurrentData.HighScore > 0)
			{
				_menuLayout.hiscoreText.Visible = true;
				_menuLayout.hiscoreText.Text = "High score: " + _saveEngine.CurrentData.HighScore;
			}

			_menuLayout.authorText.Text = $"by maksp (v{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)})";

			foreach (Button changeBindButton in _optionsLayout.bindChangeButtonsGrid.GetChildren(true, (w) => w.Id != null && w.Id.Contains("bindChange")))
			{
				(changeBindButton.FindChild(c => c is Label) as Label).Text = Enum.GetName(_saveEngine.CurrentData.Options.KeyBindings[changeBindButton.Id.Split('_')[1]]);
				changeBindButton.Click += (s, e) =>
				{
					currentEditingKeybind = (s as Button).Id.Split('_')[1];
					_desktop.KeyUp += onDesktopKeyUp;
					_optionsLayout.keyBindWindowText.Text = $"Press a keyboard button for {currentEditingKeybind} or Escape to cancel";
					_optionsLayout.keyBindWindow.ShowModal(_desktop);

				};
			}

			_gameStateModel.StateChanged += OnGameStateChanged;
		}

		private void onDesktopKeyUp(object sender, Myra.Events.GenericEventArgs<Keys> e)
		{
			_desktop.KeyUp -= onDesktopKeyUp;
			if (e.Data != Keys.Escape)
			{
				(_optionsLayout.bindChangeButtonsGrid
					.FindChildById("bindChangeButton_" + currentEditingKeybind)
					.FindChild(c => c is Label) as Label).Text = Enum.GetName(e.Data);
				_saveEngine.CurrentData.Options.KeyBindings[currentEditingKeybind] = e.Data;
			}
			_optionsLayout.keyBindWindow.Close();
		}
	}
}
