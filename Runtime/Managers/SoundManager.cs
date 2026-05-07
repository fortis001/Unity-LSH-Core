using System.Collections.Generic;
using UnityEngine;
using static LSH.Core.SoundData;

namespace LSH.Core
{
    public class SoundManager : Singleton<SoundManager>, IBootable
    {
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private SoundData _soundData;

        private Dictionary<SFXID, AudioClip> _sfxDict = new();
        private Dictionary<BGMID, AudioClip> _bgmDict = new();

        public void Init()
        {
            if (_soundData == null)
            {
                Debug.LogError("SoundData가 할당되지 않았습니다!");
                return;
            }
            foreach (var entry in _soundData.sfxList) _sfxDict[entry.id] = entry.clip;
            foreach (var entry in _soundData.bgmList) _bgmDict[entry.id] = entry.clip;
        }

        public void PlayBGM(BGMID id, bool loop = true)
        {
            if (!_bgmDict.TryGetValue(id, out AudioClip clip))
            {
                if (id != BGMID.None) Debug.LogWarning($"{id}에 해당하는 BGM이 없습니다.");
                StopBGM();
                return;
            }
            if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;
            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }

        public void PauseBGM() => _bgmSource.Pause();
        public void ResumeBGM() => _bgmSource.Play();
        public void StopBGM() => _bgmSource.Stop();

        public void PlaySFX(SFXID id)
        {
            if (_sfxDict.TryGetValue(id, out AudioClip clip))
                _sfxSource.PlayOneShot(clip);
        }
    }
}

