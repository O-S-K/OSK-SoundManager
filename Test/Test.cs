using System;
using OSK.SoundManager;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

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
            volume = new VolumeFade()
            {
                target = 1
            },
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
            volume = new VolumeFade()
            {
                init = 0,
                target = 1,
                duration = 5,
            },
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
    public void StopSoundMusic()
    {
        SoundManager.Instance.Stop(SoundType.MUSIC);
    }
    
    [Button]
    public void StopSoundSFX()
    {
        SoundManager.Instance.Stop(SoundType.SFX);
    }
    
    [Button]
    public void StopAllSounds()
    {
        SoundManager.Instance.StopAll();
    }
    
    [Button]
    public void PauseAllSounds()
    {
        SoundManager.Instance.PauseAll();
    }
    
    [Button]
    public void ResumeAllSounds()
    {
        SoundManager.Instance.ResumeAll();
    } 
    
    [Button]
    public void SetStatusSound(SoundType soundType, bool isOn)
    {
        SoundManager.Instance.SetStatusSoundType(soundType, isOn);
    }
    
    [Button]
    public void SetStatusAllSounds(bool isOn)
    {
        SoundManager.Instance.SetStatusAllSound(isOn);
    }
    
    [Button]
    public void SetVolumeSound(float volume)
    {
        SoundManager.Instance.SetAllVolume(volume);
    }
    
    [Button]
    public void SetVolumeSound(SoundType soundType, float volume)
    {
        SoundManager.Instance.SetVolume(soundType, volume);
    }
    
    [Button]
    public void  DestroyAll()
    {
        SoundManager.Instance.DestroyAll();
    }
}
