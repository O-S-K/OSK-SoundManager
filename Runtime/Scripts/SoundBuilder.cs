using UnityEngine;

namespace OSK.SoundManager
{
    public class SoundBuilder
    {
        private string _id = "";
        private AudioClip _audioClip = null;
        private SoundType _audioType = SoundType.SFX;

        private float _startTime = 0;
        private bool _loop = false;
        private float _playDelay = 0;
        
        private Transform _transform = null;
        private VolumeFade _volume = new VolumeFade(1, 1, 0);
        
        private int _priority = 128;
        private float _pitch = 1;
        private int _minDistance = 1;
        private int _maxDistance = 500;


        public SoundBuilder()
        {
        }

        public SoundBuilder SetId(string id)
        {
            _id = id;
            return this;
        }
        
        public SoundBuilder SetClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
            return this;
        }
        
        public SoundBuilder SetAudioType(SoundType audioType)
        {
            _audioType = audioType;
            return this;
        }
         
        public SoundBuilder SetStartTime(float startTime)
        {
            _startTime = startTime;
            return this;
        }

        public SoundBuilder SetLoop(bool loop)
        {
            _loop = loop;
            return this;
        }

        public SoundBuilder SetPlayDelay(float playDelay)
        {
            _playDelay = playDelay;
            return this;
        }

        public SoundBuilder SetPriority(int priority)
        {
            _priority = priority;
            return this;
        }

        public SoundBuilder SetPitch(float pitch)
        {
            _pitch = pitch;
            return this;
        }

        public SoundBuilder Set3D(Transform transform, int minDistance = 1, int maxDistance = 500)
        {
            _transform = transform;
            _minDistance = minDistance;
            _maxDistance = maxDistance;
            return this;
        }

        public SoundBuilder SetVolume(VolumeFade volume)
        {
            _volume = volume;
            return this;
        }

        public SoundBuilder PlayWithID()
        {
#if UNITY_EDITOR
            Validate();
#endif
            SoundManager.Instance.Play(_id,_volume, _startTime, _loop, _playDelay, _priority, _pitch, _transform, _minDistance, _maxDistance);
            return this;
        }

        public SoundBuilder PlayWithClip()
        {
#if UNITY_EDITOR
            Validate();
#endif
            SoundManager.Instance.PlayAudioClip(_audioClip, _audioType, _volume, _startTime, _loop, _playDelay, _priority, _pitch, _transform, _minDistance, _maxDistance);
            return this;
        }

#if UNITY_EDITOR
        private void Validate()
        {
            if (string.IsNullOrEmpty(_id) && _audioClip == null)
            {
                Debug.LogError("Either ID or AudioClip must be set.");
            }

            if (_transform == null && (_minDistance != 1 || _maxDistance != 500))
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    Debug.LogError($"Audio {_id} Transform must be set for 3D sound.");
                }
                else if (_audioClip != null)
                {
                    Debug.LogError($"Audio {_audioClip.name} Transform must be set for 3D sound.");
                }
                else
                {
                    Debug.LogError("Transform must be set for 3D sound.");
                }
            }
        }
#endif
    }
}