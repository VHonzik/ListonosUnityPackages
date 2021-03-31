using Listonos.NavigationSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

#if LISTONOS_AUDIO_SYSTEM_PRESENT
using Listonos.AudioSystem;
#endif

namespace Listonos.JamMenu
{
  public class MainMenu : MonoBehaviour
  {
    public string GameSceneName;
    public KeyCode ExitShortcut = KeyCode.Escape;

#if LISTONOS_AUDIO_SYSTEM_PRESENT
    public AudioClip MainMenuMusic;
#endif

    void Awake()
    {
      Debug.AssertFormat(GameSceneName.Length > 0, "MainMenu behavior requires GameScene string to have the name of the game scene.");
    }

    void Start()
    {
#if LISTONOS_AUDIO_SYSTEM_PRESENT
      if (MainMenuMusic != null)
      {
        AudioManager.Instance.EnqueueMusicClip(MainMenuMusic);
      }
#endif
    }

    void Update()
    {
      if (ExitShortcut != KeyCode.None && Input.GetKeyDown(ExitShortcut))
      {
        ExitButtonPressed();
      }
    }

    public void ExitButtonPressed()
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(0);
#endif
    }

    public void OptionsButtonPressed()
    {
      NavigationSystem<NavigationScreen>.Instance.CurrentScreen = NavigationScreen.Options;
    }

    public void PlayButtonPressed()
    {
      SceneManager.LoadScene(GameSceneName);
    }
  }
}
