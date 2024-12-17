using Microsoft.Xna.Framework.Audio;

namespace JABEUP_Game.Game.Controller
{
	public class SoundController
	{
		public enum SoundChannels
		{
			Music = 0,
			Effects = 1
		}

		SaveController _saveController;

		SoundEffectInstance lastPlayedSound;
		SoundEffectInstance lastPlayedMusic;

		public const float BaseVolume = 0.5f;

		public SoundController(SaveController saveController)
		{
			_saveController = saveController;
		}

		public void PlaySound(SoundEffect soundEffect, SoundChannels soundChannel)
		{
			switch (soundChannel)
			{
				case SoundChannels.Music:
					lastPlayedMusic = soundEffect.CreateInstance();
					lastPlayedMusic.IsLooped = true;
					lastPlayedMusic.Volume = _saveController.CurrentData.Options.MusicVolume / 100f * BaseVolume;
					lastPlayedMusic.Play();
					break;
				case SoundChannels.Effects:
					lastPlayedSound = soundEffect.CreateInstance();
					lastPlayedSound.Volume = _saveController.CurrentData.Options.SoundVolume / 100f * BaseVolume;
					lastPlayedSound.Play();
					break;
			}
		}

		public void StopSound(SoundChannels soundChannel)
		{
			SoundEffectInstance target = null;
			switch (soundChannel)
			{
				case SoundChannels.Music:
					target = lastPlayedMusic;
					break;
				case SoundChannels.Effects:
					target = lastPlayedSound;
					break;
			}
			target?.Stop();
		}

		public void SetPauseSound(SoundChannels soundChannel, bool isPause)
		{
			SoundEffectInstance target = null;
			switch (soundChannel)
			{
				case SoundChannels.Music:
					target = lastPlayedMusic;
					lastPlayedMusic.Volume = _saveController.CurrentData.Options.MusicVolume / 100f;
					break;
				case SoundChannels.Effects:
					target = lastPlayedSound;
					lastPlayedSound.Volume = _saveController.CurrentData.Options.SoundVolume / 100f;
					break;
			}
			if (isPause)
				target?.Pause();
			else target?.Resume();
		}
	}
}
