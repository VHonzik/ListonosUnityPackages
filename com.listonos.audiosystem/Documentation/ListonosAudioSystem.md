# Listonos' Audio System

Listonos' Audio System is a wrapper around Unity Audio System that comes with two default channels, music and sfx, and allows queueing up audio clips. It is intended for kick-starting a game dev jam projects.

## Usage

Simply grab the `AudioManager` prefab from `Assets/Prefabs` and drop it into your scene. Afterwards you can interface with using `AudioManager.Instance` singleton, such as:

```C#
using Listonos.AudioSystem;

AudioManager.Instance.PlayMusicClip(SomeAudioClip);
AudioManager.Instance.SetSfxVolume(0.5f);
```