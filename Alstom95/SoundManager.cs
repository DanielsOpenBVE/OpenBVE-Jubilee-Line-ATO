using OpenBveApi.Runtime;

namespace Alstom95;

internal static class SoundManager
{
	internal struct CarSounds
	{
		internal bool[] IsLooped;

		internal SoundHandle[] CurrentHandles;

		internal double[] LastVolume;

		internal double[] LastPitch;
	}

	private static CarSounds[] SoundHandles;

	private static PlaySoundDelegate PlaySound;

	private static PlayCarSoundDelegate PlayCarSound;

	internal static void Initialise(PlaySoundDelegate playSound, PlayCarSoundDelegate playCarSound, int numIndices)
	{
		SoundHandles = new CarSounds[Plugin.trainCars];
		for (int i = 0; i < SoundHandles.Length; i++)
		{
			SoundHandles[i].CurrentHandles = (SoundHandle[])(object)new SoundHandle[numIndices];
			SoundHandles[i].IsLooped = new bool[numIndices];
			SoundHandles[i].LastVolume = new double[numIndices];
			SoundHandles[i].LastPitch = new double[numIndices];
		}
		PlaySound = playSound;
		PlayCarSound = playCarSound;
	}

	internal static void Play(int soundIndex, double volume, double pitch, bool loop)
	{
		volume = ((volume < 0.0) ? 0.0 : volume);
		pitch = ((pitch < 0.0) ? 0.0 : pitch);
		if (soundIndex == -1)
		{
			return;
		}
		if (SoundHandles[0].CurrentHandles[soundIndex] != null)
		{
			if (SoundHandles[0].IsLooped[soundIndex] && SoundHandles[0].CurrentHandles[soundIndex].Playing)
			{
				SoundHandles[0].CurrentHandles[soundIndex].Volume = volume;
				SoundHandles[0].CurrentHandles[soundIndex].Pitch = pitch;
			}
			else if (volume == SoundHandles[0].LastVolume[soundIndex] && pitch == SoundHandles[0].LastPitch[soundIndex])
			{
				SoundHandles[0].CurrentHandles[soundIndex].Stop();
				SoundHandles[0].CurrentHandles[soundIndex] = PlaySound.Invoke(soundIndex, volume, pitch, loop);
			}
			else if (SoundHandles[0].CurrentHandles[soundIndex].Playing)
			{
				SoundHandles[0].CurrentHandles[soundIndex].Pitch = pitch;
				SoundHandles[0].CurrentHandles[soundIndex].Volume = volume;
			}
			else
			{
				SoundHandles[0].CurrentHandles[soundIndex] = PlaySound.Invoke(soundIndex, volume, pitch, loop);
			}
		}
		else
		{
			SoundHandles[0].CurrentHandles[soundIndex] = PlaySound.Invoke(soundIndex, volume, pitch, loop);
		}
		SoundHandles[0].IsLooped[soundIndex] = loop;
		SoundHandles[0].LastVolume[soundIndex] = volume;
		SoundHandles[0].LastPitch[soundIndex] = pitch;
	}

	internal static void PlayCarriage(int soundIndex, double volume, double pitch, bool loop, int carIndex)
	{
		if (carIndex > Plugin.trainCars - 1)
		{
			return;
		}
		volume = ((volume < 0.0) ? 0.0 : volume);
		pitch = ((pitch < 0.0) ? 0.0 : pitch);
		if (soundIndex == -1)
		{
			return;
		}
		if (SoundHandles[carIndex].CurrentHandles[soundIndex] != null)
		{
			if (SoundHandles[carIndex].IsLooped[soundIndex] && SoundHandles[carIndex].CurrentHandles[soundIndex].Playing)
			{
				SoundHandles[carIndex].CurrentHandles[soundIndex].Volume = volume;
				SoundHandles[carIndex].CurrentHandles[soundIndex].Pitch = pitch;
			}
			else if (volume == SoundHandles[carIndex].LastVolume[soundIndex] && pitch == SoundHandles[carIndex].LastPitch[soundIndex])
			{
				SoundHandles[carIndex].CurrentHandles[soundIndex].Stop();
				SoundHandles[carIndex].CurrentHandles[soundIndex] = PlayCarSound.Invoke(soundIndex, volume, pitch, loop, carIndex);
			}
			else if (SoundHandles[carIndex].CurrentHandles[soundIndex].Playing)
			{
				SoundHandles[carIndex].CurrentHandles[soundIndex].Pitch = pitch;
				SoundHandles[carIndex].CurrentHandles[soundIndex].Volume = volume;
			}
			else
			{
				SoundHandles[carIndex].CurrentHandles[soundIndex] = PlayCarSound.Invoke(soundIndex, volume, pitch, loop, carIndex);
			}
		}
		else
		{
			SoundHandles[carIndex].CurrentHandles[soundIndex] = PlayCarSound.Invoke(soundIndex, volume, pitch, loop, carIndex);
		}
		SoundHandles[carIndex].IsLooped[soundIndex] = loop;
		SoundHandles[carIndex].LastVolume[soundIndex] = volume;
		SoundHandles[carIndex].LastPitch[soundIndex] = pitch;
	}

	internal static void Stop(int soundIndex)
	{
		if (soundIndex != -1 && SoundHandles[0].CurrentHandles[soundIndex] != null)
		{
			SoundHandles[0].CurrentHandles[soundIndex].Stop();
			SoundHandles[0].IsLooped[soundIndex] = false;
		}
	}

	internal static void StopCarriage(int soundIndex, int carIndex)
	{
		if (carIndex <= Plugin.trainCars - 1 && soundIndex != -1 && SoundHandles[carIndex].CurrentHandles[soundIndex] != null)
		{
			SoundHandles[carIndex].CurrentHandles[soundIndex].Stop();
			SoundHandles[carIndex].IsLooped[soundIndex] = false;
		}
	}

	internal static bool IsPlaying(int soundIndex)
	{
		if (soundIndex != -1 && SoundHandles[0].CurrentHandles[soundIndex] != null && SoundHandles[0].CurrentHandles[soundIndex].Playing)
		{
			return true;
		}
		return false;
	}

	internal static bool IsPlayingCarriage(int soundIndex, int carIndex)
	{
		if (carIndex > Plugin.trainCars - 1)
		{
			return false;
		}
		if (soundIndex != -1 && SoundHandles[carIndex].CurrentHandles[soundIndex] != null && SoundHandles[carIndex].CurrentHandles[soundIndex].Playing)
		{
			return true;
		}
		return false;
	}

	internal static double GetLastPitch(int soundIndex)
	{
		if (IsPlaying(soundIndex))
		{
			return SoundHandles[0].LastPitch[soundIndex];
		}
		return -1.0;
	}

	internal static double GetLastPitchCarriage(int soundIndex, int carIndex)
	{
		if (carIndex > Plugin.trainCars - 1)
		{
			return -1.0;
		}
		if (IsPlaying(soundIndex))
		{
			return SoundHandles[carIndex].LastPitch[soundIndex];
		}
		return -1.0;
	}

	internal static double GetLastVolume(int soundIndex)
	{
		if (IsPlaying(soundIndex))
		{
			return SoundHandles[0].LastVolume[soundIndex];
		}
		return -1.0;
	}

	internal static double GetLastVolumeCarriage(int soundIndex, int carIndex)
	{
		if (carIndex > Plugin.trainCars - 1)
		{
			return -1.0;
		}
		if (IsPlaying(soundIndex))
		{
			return SoundHandles[carIndex].LastVolume[soundIndex];
		}
		return -1.0;
	}
}
