using System.Collections.Generic;
using UnityEngine;

namespace Listonos.AudioSystem
{
  public class AudioManager : SingletonMonoBehaviour<AudioManager>
  {
    public AudioSource SfxAudioSource;
    public AudioSource MusicAudioSource;

    public float DefaultVolumeForChannels;

    public bool Subtitles { get; set; }

    private AudioChannelData SfxAudioChannel;
    private AudioChannelData MusicAudioChannel;

    private Dictionary<string, AudioChannelData> CustomChannels = new Dictionary<string, AudioChannelData>();

    void Awake()
    {
      Debug.AssertFormat(SfxAudioSource, "AudioManager behavior requires SfxAudioSource to have reference to a valid AudioSource.");
      Debug.AssertFormat(MusicAudioSource, "AudioManager behavior requires MusicAudioSource to have reference to a valid AudioSource.");
      DefaultVolumeForChannels = Mathf.Clamp01(DefaultVolumeForChannels);
      SfxAudioChannel = new AudioChannelData(SfxAudioSource, DefaultVolumeForChannels);
      MusicAudioChannel = new AudioChannelData(MusicAudioSource, DefaultVolumeForChannels);
    }

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
      SfxAudioChannel.Update();
      MusicAudioChannel.Update();
      foreach (var customChannelkeyValuePair in CustomChannels)
      {
        customChannelkeyValuePair.Value.Update();
      }
    }

    public void RegisterNewCustomChannel(AudioSource audioSource, string channelName)
    {
      CustomChannels.Add(channelName, new AudioChannelData(audioSource, DefaultVolumeForChannels));
    }

    public AudioChannelData GetMusicChannelData()
    {
      return MusicAudioChannel;
    }

    public AudioChannelData GetSfxChannelData()
    {
      return SfxAudioChannel;
    }

    public AudioChannelData GetCustomChannelData(string channelName)
    {
      AudioChannelData data;
      if (CustomChannels.TryGetValue(channelName, out data))
      {
        return data;
      }

      return null;
    }

    // Fades out current music clip, if any, and plays the passed clip
    public void PlayMusicClip(AudioClip audioClip)
    {
      MusicAudioChannel.PlayFadeoutInterrupt(audioClip);
    }

    // Schedule the passed music clip to be played next
    public void EnqueueMusicClip(AudioClip audioClip)
    {
      MusicAudioChannel.Enqueue(audioClip);
    }

    // Fades out current sfx clip, if any, and plays the passed clip
    public void PlaySfxClip(AudioClip audioClip)
    {
      SfxAudioChannel.PlayFadeoutInterrupt(audioClip);
    }

    // Stops the current sfx clip, if any, and plays the passed clip
    public void PlayImmidiatelySfxClip(AudioClip audioClip)
    {
      SfxAudioChannel.PlayInterrupt(audioClip);
    }

    public void SetMusicVolume(float volume)
    {
      MusicAudioChannel.SetVolume(Mathf.Clamp01(volume));
    }

    public void SetSfxVolume(float volume)
    {
      SfxAudioChannel.SetVolume(Mathf.Clamp01(volume));
    }

    public bool IsSfxPlaying()
    {
      return SfxAudioChannel.AudioState == AudioState.Playing || SfxAudioChannel.AudioState == AudioState.FadingOut;
    }
    public bool IsMusicPlaying()
    {
      return MusicAudioChannel.AudioState == AudioState.Playing || MusicAudioChannel.AudioState == AudioState.FadingOut;
    }
  }
}

