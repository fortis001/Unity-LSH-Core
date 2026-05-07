using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSH.Core
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "LSH/Core/SoundData")]
    public class SoundData : ScriptableObject
    {
        [Serializable]
        public struct SFXEntry
        {
            [SoundIdField(SoundType.SFX)]
            public SoundId id;

            public AudioClip clip;
        }

        [Serializable]
        public struct BGMEntry
        {
            [SoundIdField(SoundType.BGM)]
            public SoundId id;

            public AudioClip clip;
        }

        public List<SFXEntry> sfxList = new();
        public List<BGMEntry> bgmList = new();
    }
}