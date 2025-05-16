using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace OSK.SoundManager
{
    public partial class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        [ShowInInspector] private List<SoundData> _listSoundData = new List<SoundData>();
        [ShowInInspector] private List<PlayingSound> _listSoundPlayings = new List<PlayingSound>();
        private Dictionary<string, Coroutine> _playingCoroutine = new Dictionary<string, Coroutine>();
        
        public static string KeyGroupPool = "AudioSound";
        
        public List<SoundData> GetListSoundData => _listSoundData;
        public List<PlayingSound> GetListSoundPlayings => _listSoundPlayings;
        public Dictionary<string, Coroutine> GetPlayingCoroutine => _playingCoroutine;
        

        [Header("Settings")]
        public ListSoundSO listSoundSo;
        public int maxCapacityMusic = 5;
        public int maxCapacitySoundEffects = 10;

        public bool IsEnableMusic = true;
        public bool IsEnableSoundSFX = true;
        public float MusicVolume = 1;
        public float SFXVolume = 1;

        private Transform _parentGroup;


        private Transform _cameraTransform;

        public Transform CameraTransform
        {
            get
            {
                if (Camera.main != null) return _cameraTransform ??= Camera.main.transform;
                else
                {
                    Debug.LogError("Camera.main is null");
                    return null;
                }
            }
            set => _cameraTransform = value;
        }

        private AudioSource _soundObject;

        private bool pauseWhenInBackground = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            OnInit();   
        }


        public void OnInit()
        {
            _listSoundData = listSoundSo.ListSoundInfos;
            if (_listSoundData == null || _listSoundData.Count == 0)
            {
                Debug.LogError("SoundInfos is empty");
                return;
            }

            _soundObject = new GameObject("AudioSource").AddComponent<AudioSource>();
            _soundObject.transform.parent = transform;
            _listSoundPlayings = new List<PlayingSound>();

            maxCapacityMusic = listSoundSo.maxCapacityMusic;
            maxCapacitySoundEffects = listSoundSo.maxCapacitySFX;
        }

#if UNITY_EDITOR
        private void OnApplicationPause(bool pause)
        {
            pauseWhenInBackground = pause;
        }
#endif


        private void Update() => CheckForStoppedMusic();

        private void CheckForStoppedMusic()
        {
            if (_listSoundPlayings == null || _listSoundPlayings.Count == 0)
                return;

#if UNITY_EDITOR
            if (pauseWhenInBackground)
                return;
#endif

            for (int i = 0; i < _listSoundPlayings.Count; i++)
            {
                var playing = _listSoundPlayings[i];
                if (playing.AudioSource != null && !playing.AudioSource.isPlaying && !playing.IsPaused)
                {
                    PoolManager.Instance.Despawn(playing.AudioSource);
                    _listSoundPlayings.RemoveAt(i--);
                }
            }
        }

        public AudioSource PlayAudioClip(AudioClip clip, SoundType soundType = SoundType.SFX, VolumeFade volume = null,
            float startTime = 0,
            bool loop = false, float delay = 0, int priority = 128, float pitch = 1,
            Transform target = null, int minDistance = 1, int maxDistance = 500)
        {
            if ((loop && !IsEnableMusic) || !IsEnableSoundSFX) return null;
            if (_listSoundPlayings.Count >= maxCapacitySoundEffects) RemoveOldestSound(SoundType.SFX);

            void PlayNow()
            {
                var source = CreateAudioSource(clip.name, clip, soundType, startTime, volume, loop,
                    priority, pitch, target, minDistance, maxDistance);
            }

            if (delay > 0)
            {
                var tween = StartCoroutine(DODelayedCall(delay, PlayNow, ignoreTimeScale: false));
                _playingCoroutine[clip.name] = tween;
            }
            else PlayNow();

            return _listSoundPlayings.LastOrDefault()?.AudioSource;
        }

        public AudioSource Play(string id, VolumeFade volume = null, float startTime = 0,
            bool loop = false, float delay = 0, int priority = 128, float pitch = 1,
            Transform target = null, int minDistance = 1, int maxDistance = 500)
        {
            if (!IsEnableMusic && !IsEnableSoundSFX) return null;

            var data = GetSoundInfo(id);
            if (data == null)
            {
                Debug.LogError("[Sound] No Sound Info with id: " + id);
                return null;
            }

            if ((data.type == SoundType.MUSIC && !IsEnableMusic) ||
                (data.type == SoundType.SFX && !IsEnableSoundSFX)) return null;

            if (data.type == SoundType.MUSIC &&
                _listSoundPlayings.Count(s => s.SoundData.type == SoundType.MUSIC) >= maxCapacityMusic)
                RemoveOldestSound(SoundType.MUSIC);
            else if (data.type == SoundType.SFX && _listSoundPlayings.Count(s => s.SoundData.type == SoundType.SFX) >=
                     maxCapacitySoundEffects)
                RemoveOldestSound(SoundType.SFX);

            void PlayNow()
            {
                var source = CreateAudioSource(id, data.audioClip, data.type, startTime, volume, loop, priority, pitch,
                    target,
                    minDistance, maxDistance);
            }

            if (delay > 0)
            {
                var coroutine = StartCoroutine(DODelayedCall(delay, PlayNow, ignoreTimeScale: false));
                _playingCoroutine[data.id] = coroutine;
            }
            else PlayNow();

            return _listSoundPlayings.LastOrDefault()?.AudioSource;
        }

        private IEnumerator DODelayedCall(float delay, System.Action action, bool ignoreTimeScale = false)
        {
            if (ignoreTimeScale)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }

            action?.Invoke();
        }

        private void RemoveOldestSound(SoundType type)
        {
            var oldest = _listSoundPlayings.FirstOrDefault(s => s.SoundData.type == type);
            if (oldest != null && oldest.AudioSource != null)
            {
                oldest.AudioSource.Stop();
                Destroy(oldest.AudioSource.gameObject);
                _listSoundPlayings.Remove(oldest);
            }
        }

        private IEnumerator DespawnAudioSource(AudioSource audioSource, float delay)
        {
            yield return new WaitForSeconds(delay);
            PoolManager.Instance.Despawn(audioSource);
        }


        private AudioSource CreateAudioSource(string id, AudioClip clip, SoundType soundType, float startTime,
            VolumeFade volume, bool loop,
            int priority, float pitch, Transform transform, int minDist, int maxDist)
        {
            var source = PoolManager.Instance.Spawn(KeyGroupPool, _soundObject, _parentGroup);
            source.Stop();
            source.name = id;
            source.clip = clip;
            source.loop = loop;

            var playing = new PlayingSound
            {
                AudioSource = source, SoundData = new SoundData { id = clip.name, audioClip = clip, type = soundType }
            };
            float volumeMultiplier = soundType == SoundType.MUSIC ? MusicVolume : SFXVolume;
            volume ??= new VolumeFade(1);
            float finalTargetVolume = volume.target;

            if (volume.duration > 0)
            {
                StartCoroutine(DoFloat(0, finalTargetVolume, volume.duration, t =>
                {
                    playing.RawVolume = t;
                    if(source) source.volume = t * volumeMultiplier;
                }));

                IEnumerator DoFloat(float from, float to, float duration, System.Action<float> onUpdate)
                {
                    float elapsed = 0;
                    while (elapsed < duration)
                    {
                        elapsed += Time.deltaTime;
                        float t = Mathf.Clamp01(elapsed / duration);
                        onUpdate(Mathf.Lerp(from, to, t));
                        yield return null;
                    }

                    onUpdate(to);
                }
            }
            else
            {
                playing.RawVolume = finalTargetVolume;
                source.volume = finalTargetVolume * volumeMultiplier;
            }


            source.priority = priority;
            source.pitch = pitch;
            source.minDistance = minDist;
            source.maxDistance = maxDist;

            if (startTime > 0) source.time = startTime;

            if (transform == null)
            {
                source.spatialBlend = 0;
                //source.transform.position = CameraTransform.position;
            }
            else
            {
                source.spatialBlend = 1;
                source.transform.position = transform.position;
            }

            /*source.outputAudioMixerGroup = data.mixerGroup;
            source.mute = data.mute;
            source.bypassEffects = data.bypassEffects;
            source.bypassListenerEffects = data.bypassListenerEffects;
            source.bypassReverbZones = data.bypassReverbZones;
            source.panStereo = data.panStereo;
            source.reverbZoneMix = data.reverbZoneMix;
            source.dopplerLevel = data.dopplerLevel;
            source.spread = data.spread;
            source.ignoreListenerVolume = data.ignoreListenerVolume;
            source.ignoreListenerPause = data.ignoreListenerPause;#1#*/

            source.Play();
            _listSoundPlayings.Add(playing);
            return source;
        }


        public SoundData GetSoundInfo(string id) => _listSoundData.FirstOrDefault(s => s.id == id);

        public SoundData GetSoundInfo(AudioClip audioClip) =>
            _listSoundData.FirstOrDefault(s => s.audioClip == audioClip);


        public void SetCameraTransform(Transform cam) => CameraTransform = cam;

        public void SetParentGroup(Transform group, bool setDontDestroy)
        {
            _parentGroup = group;
            SetGroupDontDestroyOnLoad(setDontDestroy);
        }

        public void SetGroupDontDestroyOnLoad(bool enable)
        {
            if (_parentGroup == null)
            {
                Debug.LogError("ParentGroup Sound is null. Please set it before calling this method.");
                return;
            }

            var existing = _parentGroup.GetComponent<DontDestroy>();

            if (enable)
            {
                if (existing == null)
                    existing = _parentGroup.gameObject.AddComponent<DontDestroy>();
                existing.DontDesGOOnLoad();
            }
            else
            {
                if (existing != null)
                    UnityEngine.Object.Destroy(existing);
            }
        }

        public void LogStatus()
        {
            Debug.Log("SoundManager Status");
            Debug.Log($"1.ListSoundSO: {listSoundSo}");
            Debug.Log($"2.KeyGroupPool.AudioSound: {KeyGroupPool}");
            Debug.Log($"3.CameraTransform: {CameraTransform}");
            Debug.Log($"4.ParentGroup: {_parentGroup}");

            Debug.Log($"AudioListener: {AudioListener.pause}");
            Debug.Log($"5.IsEnableMusic: {IsEnableMusic}");
            Debug.Log($"6.IsEnableSoundSFX: {IsEnableSoundSFX}");

            Debug.Log($"7.MaxCapacityMusic: {maxCapacityMusic}");
            Debug.Log($"8.MaxCapacitySoundEffects: {maxCapacitySoundEffects}");

            Debug.Log($"9.ListSoundInfos: {_listSoundData.Count}");
            Debug.Log($"10.ListMusicInfos: {_listSoundPlayings.Count}");

            for (int i = 0; i < _listSoundPlayings.Count; i++)
            {
                Debug.Log($"_listMusicInfos[{i}]: {_listSoundPlayings[i].SoundData.id}");
            }

            Debug.Log("End SoundManager Status");
        }
    }
}