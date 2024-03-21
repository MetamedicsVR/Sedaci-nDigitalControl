using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviourInstance<AudioManager>
{
	private Dictionary<AudioName, AudioSource> uniqueAudios = new Dictionary<AudioName, AudioSource>();
	private Dictionary<AudioName, AudioClip> audioClips = new Dictionary<AudioName, AudioClip>();
	public AudioSource backgroundMusic;

	public AudioClip GetBackGroundMusic()
	{
		return backgroundMusic.clip;
	}

	public void SetBackGroundMusic(AudioName name)
	{
		AudioClip clip = GetAudioClip(name);
		if (clip)
		{
			backgroundMusic.clip = clip;
			backgroundMusic.Play();
		}
	}

	public void StopBackGroundMusic()
	{
		backgroundMusic.Stop();
	}

	public float TemporalAudio(AudioName name, Transform t, Vector3 p)
	{
		AudioClip clip = GetAudioClip(name);
		if (clip)
		{
			GameObject audioObject = new GameObject("TemporalAudio: " + name);
			audioObject.transform.parent = t;
			audioObject.transform.position = p;
			AudioSource audioSource = audioObject.AddComponent<AudioSource>();
			audioSource.clip = clip;
			audioSource.Play();
			AutoDestroy autoDestroy = audioObject.AddComponent<AutoDestroy>();
			autoDestroy.totalTimeToDestroy = clip.length;
			autoDestroy.StartCountDown();
			return clip.length;
		}
		return 0;
	}

	public float TemporalAudio(AudioName name, Transform t)
	{
		return TemporalAudio(name, t, t.position);
	}

	public float TemporalAudio(AudioName name, Vector3 p)
	{
		return TemporalAudio(name, null, p);
	}

	public float UniqueAudio(AudioName name, Transform t, Vector3 p)
	{
		AudioClip clip = GetAudioClip(name);
		if (clip)
		{
			AudioSource uniqueAudio = null;
			if (uniqueAudios.ContainsKey(name))
			{
				uniqueAudio = uniqueAudios[name];
			}
			if (!uniqueAudio)
			{
				GameObject audioObject = new GameObject("UniqueAudio: " + name);
				uniqueAudio = audioObject.AddComponent<AudioSource>();
				uniqueAudios[name] = uniqueAudio;
			}
			if (!uniqueAudio.isPlaying)
			{
				uniqueAudio.transform.parent = t;
				uniqueAudio.transform.position = p;
				uniqueAudio.Play();
				return clip.length;
			}
			else
			{
				return clip.length - uniqueAudio.time;
			}
		}
		return 0;
	}

	public float UniqueAudio(AudioName name, Transform t)
	{
		return UniqueAudio(name, t, t.position);
	}

	public float UniqueAudio(AudioName name, Vector3 p)
	{
		return UniqueAudio(name, null, p);
	}

	public AudioClip GetAudioClip(AudioName name)
	{
		if (audioClips.ContainsKey(name))
		{
			return audioClips[name];
		}
		AudioClip clip = Resources.Load<AudioClip>("Audio/" + name);
		if (clip)
		{
			audioClips[name] = clip;
		}
		return clip;
	}

	public enum AudioName
	{
		Explanation,
		SeahorsesExplanation,
		SeaHorsesStretch1,
		SeaHorsesStretch2,
		SeaHorsesStretch3,
		SeaHorsesStretch4,
		SeaHorsesStretch5,
		SeaHorsesLetGo1,
		SeaHorsesLetGo2,
		SeaHorsesLetGo3,
		SeaHorsesLetGo4,
		SeaHorsesLetGo5,
		BlowfishExplanation,
		BlowfishInspiration1,
		BlowfishInspiration2,
		BlowfishInspiration3,
		BlowfishInspiration4,
		BlowfishInspiration5,
		BlowFishExpiration1,
		BlowFishExpiration2,
		BlowFishExpiration3,
		BlowFishExpiration4,
		BlowFishExpiration5,
		Turtles,
		Dolphins

	}
}