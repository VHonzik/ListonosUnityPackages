using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Listonos.AudioSystem
{
  public enum AudioState
  {
    NoClip,
    Playing,
    Paused,
    FadingOut
  }

  public class AudioChannelData
  {
    public AudioSource AudioSource { get; private set; }
    private Queue<AudioClip> AudioClips { get; set; } = new Queue<AudioClip>();

    public AudioState AudioState { get; private set; } = AudioState.NoClip;

    public float DefaultVolume { get; private set; }

    public AudioChannelData(AudioSource audioSource, float defaultVolume)
    {
      AudioSource = audioSource;
      DefaultVolume = defaultVolume;
      AudioSource.volume = DefaultVolume;
    }

    public void Update()
    {
      // Finished playing?
      if (AudioState == AudioState.Playing && AudioSource.clip != null && AudioSource.time >= AudioSource.clip.length)
      {
        if (AudioClips.Count > 0)
        {
          AudioState = AudioState.Playing;
          AudioSource.clip = AudioClips.Dequeue();
          AudioSource.Play();
        }
        else
        {
          AudioState = AudioState.NoClip;
          AudioSource.clip = null;
        }
      }
      // Fading out
      else if (AudioState == AudioState.FadingOut)
      {
        AudioSource.volume = Mathf.Clamp01(AudioSource.volume - Time.deltaTime);
        if (AudioSource.volume <= 0.0f)
        {
          if (AudioClips.Count > 0)
          {
            AudioSource.Stop();
            AudioSource.volume = DefaultVolume;
            AudioState = AudioState.Playing;
            AudioSource.clip = AudioClips.Dequeue();
            AudioSource.Play();
          }
          else
          {
            AudioState = AudioState.NoClip;
            AudioSource.Stop();
            AudioSource.clip = null;
            AudioSource.volume = DefaultVolume;
          }
        }
      }
    }

    public void SetVolume(float volume)
    {
      if (AudioState != AudioState.FadingOut || AudioSource.volume > volume)
      {
        AudioSource.volume = volume;
      }
      DefaultVolume = volume;
    }

    public void Pause()
    {
      if (AudioState == AudioState.Playing)
      {
        AudioSource.Pause();
        AudioState = AudioState.Paused;
      }
    }

    public void Unpause()
    {
      if (AudioState == AudioState.Paused)
      {
        AudioSource.UnPause();
        AudioState = AudioState.Playing;
      }
    }

    // Fades out the current clip, if any, and clears the queue
    public void FadeoutInterrupt()
    {
      AudioClips.Clear();
      if (AudioState == AudioState.Playing)
      {
        AudioState = AudioState.FadingOut;
      }
      else if (AudioState == AudioState.Paused || AudioState == AudioState.NoClip)
      {
        AudioState = AudioState.NoClip;
        AudioSource.clip = null;
      }
    }

    // Schedule the passed clip to be played next
    public void Enqueue(AudioClip audioClip)
    {
      AudioClips.Enqueue(audioClip);
      if (AudioState == AudioState.NoClip)
      {
        AudioState = AudioState.Playing;
        AudioSource.clip = AudioClips.Dequeue();
        AudioSource.Play();
      }
    }

    // Fades out the current clip, if any, clears the queue and play the passed clip next
    public void PlayFadeoutInterrupt(AudioClip audioClip)
    {
      AudioClips.Clear();
      AudioClips.Enqueue(audioClip);
      if (AudioState == AudioState.Playing)
      {
        AudioState = AudioState.FadingOut;
      }
      else if (AudioState == AudioState.Paused || AudioState == AudioState.NoClip)
      {
        AudioState = AudioState.Playing;
        AudioSource.clip = AudioClips.Dequeue();
        AudioSource.Play();
      }
    }

    // Abruptly stop the current clip, if any, and start playing the passed clip
    public void PlayInterrupt(AudioClip audioClip)
    {
      AudioClips.Clear();
      AudioClips.Enqueue(audioClip);

      if (AudioState == AudioState.Playing || AudioState == AudioState.Paused)
      {
        AudioSource.Stop();
      }
      else if (AudioState == AudioState.FadingOut)
      {
        AudioSource.Stop();
        AudioSource.volume = DefaultVolume;
      }


      AudioState = AudioState.Playing;
      AudioSource.clip = AudioClips.Dequeue();
      AudioSource.Play();
    }
  }
}