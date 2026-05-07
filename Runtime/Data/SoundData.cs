using System;
using System.Collections.Generic;
using UnityEngine;


namespace LSH.Core
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Core/SoundData")]
    public class SoundData : ScriptableObject
    {
        public enum SFXID
        {
            None = 0,

            // ---  UI & Navigation ---
            UI_Btn_Click = 100,
            UI_Btn_Hover = 101,
            UI_Btn_Confirm = 102,
            UI_Menu_Back = 110,
            UI_Menu_Open = 111,
            UI_Error = 190,

            // --- 
        }

        public enum BGMID
        {
            None = 0,

            // ---
        }


        [Serializable]
        public struct SFXEntry
        {
            public SFXID id;
            public AudioClip clip;
        }

        [Serializable]
        public struct BGMEntry
        {
            public BGMID id;
            public AudioClip clip;
        }

        public List<SFXEntry> sfxList;
        public List<BGMEntry> bgmList;
    }
}


