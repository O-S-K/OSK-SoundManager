using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OSK.SoundManager
{
    [CreateAssetMenu(fileName = "ListSoundSO", menuName = "OSK/Sound/ListSoundSO")]
    public class ListSoundSO : ScriptableObject
    {
        [HideInInspector]
        public string filePathSoundID = "Assets";
        
        [Space]
        [Title("Max Capacity")]
        public int maxCapacityMusic = 5;
        public int maxCapacitySFX = 25;
        
        [Space]
        [Title("List Sound Infos")]
        [TableList,SerializeField] 
        private List<SoundData> _listSoundInfos = new List<SoundData>();
        public List<SoundData> ListSoundInfos => _listSoundInfos;


#if UNITY_EDITOR 
        private void OnValidate()
        {
            // check unique audio clip name in the list
            var audioClipNames = new List<string>();
            foreach (var soundInfo in _listSoundInfos)
            {
                if(soundInfo.audioClip == null || string.IsNullOrEmpty(soundInfo.audioClip.name))
                    continue;
                if (audioClipNames.Contains(soundInfo.audioClip.name))
                {
                    Debug.LogError(
                        $"Audio Clip Name {soundInfo.audioClip.name} exists in the list. Please remove it or rename it.");
                }
                else
                { 
                    audioClipNames.Add(soundInfo.audioClip.name);
                }
            }
        }
#endif
    }
}