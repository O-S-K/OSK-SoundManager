using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace OSK.SoundManager
{
    [System.Serializable]
    [InlineProperty]
    public class SoundSetup
    {
        [LabelWidth(100)] public string id;

        [LabelWidth(100)] public AudioClip audioClip = null;

        [EnumToggleButtons] [LabelWidth(100)] public SoundType type = SoundType.SFX;

        [LabelWidth(100)] public float startTime = 0;
        [LabelWidth(100)] public bool loop = false;

        [FoldoutGroup("Advanced", expanded: false)] [LabelText("Volume")] [LabelWidth(100)]
        public VolumeFade volume = VolumeFade.Default;

        [FoldoutGroup("Advanced")]
        [PropertyTooltip("Higher = more important")]
        [PropertyRange(0, 256)]
        [LabelWidth(100)]
        public int priority = 128;

        [FoldoutGroup("Advanced")] [Range(0.1f, 3f)] [LabelWidth(100)]
        public float pitch = 1;

        [FoldoutGroup("Advanced")] [PropertyRange(0, 10)] [LabelWidth(100)]
        public float playDelay = 0;

        [FoldoutGroup("3D Settings")] [LabelWidth(100)]
        public Transform transform = null;

        [FoldoutGroup("3D Settings")] [LabelWidth(100)] [MinValue(0)]
        public int minDistance = 1;

        [FoldoutGroup("3D Settings")] [LabelWidth(100)] [MinValue(0)]
        public int maxDistance = 500;

        public SoundSetup(string id = "", AudioClip audioClip = null, SoundType type = SoundType.SFX,
            float startTime = 0, bool loop = false, VolumeFade volume = null, float playDelay = 0, int priority = 128,
            float pitch = 1, Transform transform = null, int minDistance = 1, int maxDistance = 500)
        {
            this.id = id;
            this.audioClip = audioClip;
            this.type = type;
            this.startTime = startTime;
            this.loop = loop;
            this.volume = volume ?? new VolumeFade();
            this.playDelay = playDelay;
            this.priority = priority;
            this.pitch = pitch;
            this.transform = transform;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
        }

        public SoundSetup()
        {
            id = "";
            audioClip = null;
            type = SoundType.SFX;
            startTime = 0;
            loop = false;
            volume = VolumeFade.Default;
            playDelay = 0;
            priority = 128;
            pitch = 1;
            transform = null;
            minDistance = 1;
            maxDistance = 500;
        }
    }

    [System.Serializable]
    public class VolumeFade
    {
        [LabelText("Init Volume")] [HorizontalGroup("Volume")] [LabelWidth(100)] [Min(0)]
        public float init = 0;

        [HorizontalGroup("Volume")] [LabelWidth(100)] [Min(0)]
        public float target = 1;

        [HorizontalGroup("Volume")] [LabelWidth(100)] [Min(0)]
        public float duration = 0;

        public VolumeFade(float init = 0, float target = 1, float duration = 0)
        {
            this.init = init;
            this.target = target;
            this.duration = duration;
        }

        public static VolumeFade Default => new VolumeFade(0, 1, 0);
    }

    public partial class SoundManager
    {
        #region With SoundSetup

        /// <summary>
        ///  Play a sound with Id in list Sound SO using SoundSetup
        /// </summary>
        /// <param name="soundSetup"></param>
        /// <returns></returns>
        public AudioSource PlayID(SoundSetup soundSetup)
        {
            return Play(soundSetup.id, soundSetup.volume, soundSetup.startTime, soundSetup.loop,
                soundSetup.playDelay,
                soundSetup.priority,
                soundSetup.pitch,
                soundSetup.transform, soundSetup.minDistance, soundSetup.maxDistance);
        }

        /// <summary>
        ///  Play a sound with ClipAudio using SoundSetup
        /// </summary>
        /// <param name="soundSetup"></param>
        /// <returns></returns>
        public AudioSource PlayClip(SoundSetup soundSetup)
        {
            return PlayAudioClip(soundSetup.audioClip, soundSetup.type, soundSetup.volume, soundSetup.startTime,
                soundSetup.loop, soundSetup.playDelay,
                soundSetup.priority,
                soundSetup.pitch,
                soundSetup.transform, soundSetup.minDistance, soundSetup.maxDistance);
        }

        #endregion

        #region Stop and Pause

        public void StopAll()
        {
            for (int i = 0; i < _listSoundPlayings.Count; i++)
            {
                _listSoundPlayings[i].AudioSource.Stop();
                PoolManager.Instance.Despawn(_listSoundPlayings[i].AudioSource);
                _listSoundPlayings.RemoveAt(i);
                i--;
            }

            StopAllPendingAudios();
        }

        public void Stop(string id)
        {
            for (int i = 0; i < _listSoundPlayings.Count; i++)
            {
                if (_listSoundPlayings[i].SoundData.id == id)
                {
                    _listSoundPlayings[i].AudioSource.Stop();
                    PoolManager.Instance.Despawn(_listSoundPlayings[i].AudioSource);
                    _listSoundPlayings.RemoveAt(i);
                    i--;
                }
            }

            StopPendingAudio(id);
        }

        public void Stop(AudioClip clip)
        {
            for (int i = 0; i < _listSoundPlayings.Count; i++)
            {
                if (_listSoundPlayings[i].AudioSource.clip == clip)
                {
                    _listSoundPlayings[i].AudioSource.Stop();
                    PoolManager.Instance.Despawn(_listSoundPlayings[i].AudioSource);
                    _listSoundPlayings.RemoveAt(i);
                    i--;
                }
            }

            StopPendingAudio(clip.name);
        }


        public void Stop(SoundType type)
        {
            for (int i = 0; i < _listSoundPlayings.Count; i++)
            {
                if (_listSoundPlayings[i].SoundData.type == type)
                {
                    _listSoundPlayings[i].AudioSource.Stop();
                    PoolManager.Instance.Despawn(_listSoundPlayings[i].AudioSource);
                    _listSoundPlayings.RemoveAt(i);
                    i--;
                }
            }

            StopPendingAudio(type.ToString());
        }

        public void StopPendingAudio(string clipId)
        {
            if (_playingCoroutine.TryGetValue(clipId, out var coroutine))
            {
                if (coroutine != null) StopCoroutine(coroutine);
                _playingCoroutine.Remove(clipId);
            }
        }

        public void StopAllPendingAudios()
        {
            foreach (var coroutine in _playingCoroutine.Values)
            {
                if (coroutine != null) StopCoroutine(coroutine);
            }
            _playingCoroutine.Clear();
        }


        public void PauseAll()
        {
            foreach (var playingSound in _listSoundPlayings)
            {
                playingSound.AudioSource.Pause();
                playingSound.IsPaused = true;
            }
        }

        public void Pause(SoundType type)
        {
            foreach (var playingSound in _listSoundPlayings)
            {
                if (playingSound.SoundData.type == type)
                {
                    playingSound.IsPaused = true;
                    playingSound.AudioSource.Pause();
                }
            }
        }

        public void ResumeAll()
        {
            foreach (var playingSound in _listSoundPlayings)
            {
                playingSound.IsPaused = false;
                playingSound.AudioSource.UnPause();
            }
        }

        public void Resume(SoundType type)
        {
            foreach (var playingSound in _listSoundPlayings)
            {
                if (playingSound.SoundData.type == type)
                {
                    playingSound.AudioSource.UnPause();
                    playingSound.IsPaused = false;
                }
            }
        }

        #endregion

        #region Status

        public void SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            if (_listSoundPlayings == null || _listSoundPlayings.Count == 0) return;
            foreach (var playing in _listSoundPlayings.Where(playing => playing.AudioSource != null))
            {
                playing.AudioSource.outputAudioMixerGroup = mixerGroup;
            }
        }

        public void SetStatusSoundType(SoundType type, bool isOn)
        {
            switch (type)
            {
                case SoundType.MUSIC:
                    IsEnableMusic = isOn;
                    if (IsEnableMusic)
                        Resume(SoundType.MUSIC);
                    else
                        Pause(SoundType.MUSIC);
                    break;
                case SoundType.SFX:
                    IsEnableSoundSFX = isOn;
                    if (IsEnableSoundSFX)
                        Resume(SoundType.SFX);
                    else
                        Pause(SoundType.SFX);
                    break;
            }
        }


        public void SetStatusAllSound(bool isOn)
        {
            IsEnableMusic = isOn;
            IsEnableSoundSFX = isOn;

            if (!isOn)
            {
                PauseAll();
            }
            else
            {
                ResumeAll();
            }
        }

        public void SetAllVolume(float volume)
        {
            MusicVolume = volume;
            SFXVolume = volume;

            foreach (var s in _listSoundPlayings)
            {
                float multiplier = s.SoundData.type == SoundType.MUSIC ? MusicVolume : SFXVolume;
                s.AudioSource.volume = s.RawVolume * multiplier;
            }
        }

        public void SetVolume(SoundType type, float volume)
        {
            if (type == SoundType.MUSIC) MusicVolume = volume;
            else if (type == SoundType.SFX) SFXVolume = volume;

            foreach (var s in _listSoundPlayings)
            {
                if (s.SoundData.type == type)
                {
                    float multiplier = type == SoundType.MUSIC ? MusicVolume : SFXVolume;
                    s.AudioSource.volume = s.RawVolume * multiplier;
                }
            }
        }

        public AudioClip GetAudioClipInSO(string id)
        {
            foreach (var t in _listSoundData)
            {
                if (t.id == id) return t.audioClip;
            }

            return null;
        }

        public AudioClip GetAudioClipInSO(AudioClip audioClip)
        {
            foreach (var t in _listSoundData)
            {
                if (t.audioClip == audioClip) return t.audioClip;
            }

            return null;
        }

        public AudioClip GetAudioClipOnScene(string id)
        {
            foreach (var t in _listSoundPlayings)
            {
                if (t.SoundData.id == id) return t.AudioSource.clip;
            }

            return null;
        }

        #endregion

        #region Dispose

        public void Despawn(AudioSource audioSource, float delay = 0)
        {
            StartCoroutine(DespawnAudioSource(audioSource, delay));
            StopPendingAudio(audioSource.clip.name);
        }

        public void DespawnMusicID(string id, float delay = 0)
        {
            for (int i = 0; i < _listSoundPlayings.Count; i++)
            {
                if (_listSoundPlayings[i].SoundData.id == id)
                {
                    StartCoroutine(DespawnAudioSource(_listSoundPlayings[i].AudioSource, delay));
                    _listSoundPlayings.RemoveAt(i);
                    i--;
                }
            }
        }

        public void DestroyAll()
        {
            for (int i = _listSoundPlayings.Count - 1; i >= 0; i--)
            {
                Destroy(_listSoundPlayings[i].AudioSource);
            }

            StopAllPendingAudios();

            _listSoundPlayings.Clear();
            _playingCoroutine.Clear();
            PoolManager.Instance.DestroyAllInGroup(KeyGroupPool);
        }

        #endregion
    }
}