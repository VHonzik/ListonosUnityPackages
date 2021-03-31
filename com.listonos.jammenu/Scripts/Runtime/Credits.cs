using System;
using UnityEngine;

#if LISTONOS_AUDIO_SYSTEM_PRESENT
using Listonos.AudioSystem;
#endif

namespace Listonos.JamMenu
{
  public class Credits : MonoBehaviour
  {
    public string JamPageURL;
#if LISTONOS_AUDIO_SYSTEM_PRESENT
    public AudioClip CreditsMusic;
#endif

    private bool urlValid = false;

    void Awake()
    {
      // Source: https://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
      Uri uriResult;
      urlValid = Uri.TryCreate(JamPageURL, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
      Debug.AssertFormat(urlValid, "Credits behavior requires JamPageURL string to have valid URL of the game jam page.");
    }

    void Start()
    {
#if LISTONOS_AUDIO_SYSTEM_PRESENT
      if (CreditsMusic != null)
      {
        AudioManager.Instance.PlayMusicClip(CreditsMusic);
      }
#endif
    }

    public void ExitButtonPressed()
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(0);
#endif
    }

    public void RateButtonPressed()
    {
      if (urlValid)
      {
        Application.OpenURL(JamPageURL);
      }
    }
  }
}
