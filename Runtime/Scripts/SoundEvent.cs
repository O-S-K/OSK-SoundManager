using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OSK.SoundManager
{
    public class SoundEvent : MonoBehaviour
    {
        [Tooltip("ID or Clip")]
        [LabelWidth(100)]
        [EnumToggleButtons]
        public enum ETypePlay
        {
            ID,
            Clip
        }

        public ETypePlay typePlay;

        [LabelText("Setting")] 
        public SoundSetup[] soundSetupInfo = new SoundSetup[1];

        
        
        [Button]
        public void Play()
        {
            foreach (var soundSetup in soundSetupInfo)
            {
                if (typePlay == ETypePlay.ID) SoundManager.Instance.PlayID(soundSetup);
                else SoundManager.Instance.PlayClip(soundSetup);
            }
        }
       

        [Button]
        public void Stop()
        {
            foreach (var soundSetup in soundSetupInfo)
            {
                if (typePlay == ETypePlay.ID) SoundManager.Instance.Stop(soundSetup.id);
                else SoundManager.Instance.Stop(soundSetup.audioClip);
            }
        }
    }
}