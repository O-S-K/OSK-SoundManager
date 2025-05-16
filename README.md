## 🔊 SoundManager

**SoundManager** is responsible for controlling the entire sound system in the game, including **background music (Music)**, **sound effects (SFX)**, and **special sound events**.

It supports features such as: play, stop, pause, resume, adjust volume, mute/unmute by sound type, and control by predefined ID.

---

## 🎮 Example Usage

```csharp
using OSK;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{

    private void Awake()
    {
        // Set for container avoid object pool when switching scenes
        var goContainer = new GameObject("SoundManager");
        SoundManager.Instance.SetParentGroup(goContainer.transform, true);

        // Set for object pool and max capacity
        SoundManager.Instance.maxCapacityMusic = 5;
        SoundManager.Instance.maxCapacitySoundEffects = 10;
        
        // Set and get status sound
        SoundManager.Instance.IsEnableMusic = true;
        SoundManager.Instance.IsEnableSoundSFX = true;
    }

    [Button]
    public void PlaySoundSFX()
    {
        SoundManager.Instance.PlayID(new SoundSetup()
        {
            id = SoundID.SFX.ToString(),
            loop = false,
            volume = new VolumeFade() { target = 1 },
            pitch = Random.Range(0.8f, 1.2f),
            playDelay = 0
        });
    }

    [Button]
    public void PlaySoundMusic()
    {
        SoundManager.Instance.PlayID(new SoundSetup()
        {
            id = SoundID.Music.ToString(),
            loop = true,
            volume = new VolumeFade() { init = 0, target = 1, duration = 5 },
            pitch = Random.Range(0.8f, 1.2f),
            playDelay = 0
        });
    }

    public AudioClip playClip;
    
    [Button]
    public void PlaySFXWithClip()
    {
        SoundManager.Instance.PlayClip(new SoundSetup()
        {
            audioClip = playClip
        });
    }
    
    
    public Transform playTransform3D;
    [Button]
    public void PlaySFXWith3DClip()
    {
        SoundManager.Instance.PlayClip(new SoundSetup()
        {
            audioClip = playClip,
            loop = false,
            volume = new VolumeFade()
            {
                target = 1
            },
            pitch = Random.Range(0.8f, 1.2f),
            playDelay = 0,
            transform = playTransform3D,
            minDistance = 1,
            maxDistance = 100,
        });
    }

    [Button]
    public void PlayWithBuilder()
    {
        new SoundBuilder()
            .SetId(nameof(SoundID.SFX))
            .SetVolume(new VolumeFade(1))
            .SetLoop(false)
            .SetPitch(Random.Range(0.8f, 1.2f));
    }

    [Button]
    public void StopSoundMusic() => SoundManager.Instance.Stop(SoundType.MUSIC);

    [Button]
    public void StopSoundSFX() => SoundManager.Instance.Stop(SoundType.SFX);

    [Button]
    public void StopAllSounds() => SoundManager.Instance.StopAll();

    [Button]
    public void PauseAllSounds() => SoundManager.Instance.PauseAll();

    [Button]
    public void ResumeAllSounds() => SoundManager.Instance.ResumeAll();

    [Button]
    public void SetStatusSound(SoundType type, bool isOn) =>
        SoundManager.Instance.SetStatusSoundType(type, isOn);

    [Button]
    public void SetStatusAllSounds(bool isOn) =>
        SoundManager.Instance.SetStatusAllSound(isOn);

    [Button]
    public void SetVolumeSound(float volume) =>
        SoundManager.Instance.SetAllVolume(volume);

    [Button]
    public void SetVolumeSound(SoundType type, float volume) =>
        SoundManager.Instance.SetVolume(type, volume);

    [Button]
    public void DestroyAll() => SoundManager.Instance.DestroyAll();
}

## ✅ Outstanding Features

- Clearly distinguish between **background music** and **sound effects**.
- Support **playback by ID** defined in `ListSoundSO`.
- Adjust **volume** per type or globally.
- Control **stop**, **pause**, **resume** for all or specific types.
- Supports advanced settings like `VolumeFade`, `PlayDelay`, and `PitchRandomize`.

---

## 📦 1. Install Dependencies

- **Odin Inspector**:  
  [https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)

- **Pooling System**:  
  [PoolingManager](https://github.com/O-S-K/OSK-PoolManager.git)


## 🛠️ ScriptableObject Setup

To create sound lists used by the system:


🛠️ ScriptableObject Setup
Create ListSoundSO or ListMusicSO via the menu: 
Assets → Create → OSK → ListSoundSO / ListMusicSO 

Each entry in the list allows you to:
- Define a unique **ID**
- Assign an `AudioClip`
- Set `SoundType` (SFX or Music)
- Configure default **volume**, **loop**, **playOnAwake**, etc.

---

## 📌 Tips

- Use `VolumeFade` to create smooth **fade in/out** effects for background music.
- Randomize pitch with `Random.Range()` to make repetitive sounds feel more natural.
- The system **automatically manages AudioSource pooling** to reduce garbage collection and boost performance.
- Combine `ListSoundSO` with the `SoundID` enum for centralized audio management.

---

## 📞 Support

- **Email**:  [gamecoding1999@gmail.com](mailto:gamecoding1999@gmail.com)
- **Facebook**: [OSK](https://www.facebook.com/xOskx/)
