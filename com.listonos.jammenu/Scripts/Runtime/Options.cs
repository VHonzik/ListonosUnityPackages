using Listonos.NavigationSystem;
using UnityEngine;

#if LISTONOS_AUDIO_SYSTEM_PRESENT
using Listonos.AudioSystem;
#endif

namespace Listonos.JamMenu
{
  public class Options : MonoBehaviour
  {
    public UnityEngine.UI.Toggle FullscreenToggle;

#if LISTONOS_AUDIO_SYSTEM_PRESENT
    public AudioClip SfxVolumeTestClip;
    public AudioClip MusicVolumeTestClip;
#endif
    void Awake()
    {
      Debug.AssertFormat(FullscreenToggle, "Options behavior requires FullscreenToggle to have reference to a valid Toggle element.");
    }

    // Start is called before the first frame update
    void Start()
    {
      FullscreenToggle.isOn = Screen.fullScreen;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FullScreenToggleValueChanged(bool isOn)
    {
      Screen.fullScreen = isOn;
    }

    public void SubtitlesToggleValueChanged(bool isOn)
    {
#if LISTONOS_AUDIO_SYSTEM_PRESENT
      AudioManager.Instance.Subtitles = isOn;
#endif
    }

    public void MusicVolumeSliderChanged(float value)
    {
#if LISTONOS_AUDIO_SYSTEM_PRESENT
      AudioManager.Instance.SetMusicVolume(value);
      if (MusicVolumeTestClip != null && !AudioManager.Instance.IsMusicPlaying())
      {
        AudioManager.Instance.PlayMusicClip(MusicVolumeTestClip);
      }
#endif
    }

    public void SfxVolumeSliderChanged(float value)
    {
#if LISTONOS_AUDIO_SYSTEM_PRESENT
      AudioManager.Instance.SetSfxVolume(value);
      if (SfxVolumeTestClip != null && !AudioManager.Instance.IsSfxPlaying())
      {
        AudioManager.Instance.PlaySfxClip(SfxVolumeTestClip);
      }
#endif
    }

    public void BackButtonPressed()
    {
      NavigationSystem<NavigationScreen>.Instance.CurrentScreen = NavigationScreen.MainMenu;
    }
  }
}
