using UnityEngine;

#if LISTONOS_AUDIO_SYSTEM_PRESENT
using Listonos.AudioSystem;
#endif


public class ButtonWithAudio : MonoBehaviour
{
  public AudioClip PressedAudio;

  public void ButtonPressed()
  {
#if LISTONOS_AUDIO_SYSTEM_PRESENT
    if (PressedAudio != null)
    {
      AudioManager.Instance.TryPlayingSfxClip(PressedAudio);
    }
#endif
  }
}
