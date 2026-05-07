using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSH.Core
{
    public class SoundManager : Singleton<SoundManager>, IBootable
    {
        [SerializeField] private SoundData _soundData;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _bgmSource;

        private readonly Dictionary<int, AudioClip> _sfxById = new();
        private readonly Dictionary<int, AudioClip> _bgmById = new();

        public void Init()
        {
            if (_soundData == null)
            {
                Debug.LogError("SoundData가 할당되지 않았습니다!", this);
                return;
            }

            _sfxById.Clear();
            _bgmById.Clear();

            foreach (var entry in _soundData.sfxList)
            {
                if (entry.clip == null || entry.id.IsEmpty)
                    continue;

                if (_sfxById.ContainsKey(entry.id.Id))
                {
                    Debug.LogWarning(
                        $"중복된 SFX ID가 있습니다. id: {entry.id.Id}, name: {entry.id.Name}",
                        this);
                }

                _sfxById[entry.id.Id] = entry.clip;
            }

            foreach (var entry in _soundData.bgmList)
            {
                if (entry.clip == null || entry.id.IsEmpty)
                    continue;

                if (_bgmById.ContainsKey(entry.id.Id))
                {
                    Debug.LogWarning(
                        $"중복된 BGM ID가 있습니다. id: {entry.id.Id}, name: {entry.id.Name}",
                        this);
                }

                _bgmById[entry.id.Id] = entry.clip;
            }
        }

        public void PlaySFX(SoundId soundId)
        {
            if (soundId.IsEmpty)
            {
                Debug.LogWarning("SFX ID가 비어 있습니다.", this);
                return;
            }

            PlaySFX(soundId.Id);
        }

        public void PlaySFX(int id)
        {
            if (!_sfxById.TryGetValue(id, out AudioClip clip))
            {
                Debug.LogWarning($"SFX not found. id: {id}", this);
                return;
            }

            PlaySFXClip(clip);
        }

        public void PlaySFX<TEnum>(TEnum id)
            where TEnum : struct, Enum
        {
            if (!ValidateSoundEnum<TEnum>(SoundType.SFX))
                return;

            PlaySFX(Convert.ToInt32(id));
        }

        public void PlayBGM(SoundId soundId)
        {
            if (soundId.IsEmpty)
            {
                Debug.LogWarning("BGM ID가 비어 있습니다.", this);
                return;
            }

            PlayBGM(soundId.Id);
        }

        public void PlayBGM(int id)
        {
            if (!_bgmById.TryGetValue(id, out AudioClip clip))
            {
                Debug.LogWarning($"BGM not found. id: {id}", this);
                return;
            }

            PlayBGMClip(clip);
        }

        public void PlayBGM<TEnum>(TEnum id)
            where TEnum : struct, Enum
        {
            if (!ValidateSoundEnum<TEnum>(SoundType.BGM))
                return;

            PlayBGM(Convert.ToInt32(id));
        }

        private static bool ValidateSoundEnum<TEnum>(SoundType expectedKind)
            where TEnum : struct, Enum
        {
            Type enumType = typeof(TEnum);

            SoundIdEnumAttribute attribute =
                Attribute.GetCustomAttribute(enumType, typeof(SoundIdEnumAttribute))
                as SoundIdEnumAttribute;

            if (attribute == null)
            {
                Debug.LogError(
                    $"{enumType.Name}에는 [{nameof(SoundIdEnumAttribute)}]가 붙어 있어야 합니다.");
                return false;
            }

            if (attribute.Type != expectedKind)
            {
                Debug.LogError(
                    $"{enumType.Name}은 SoundKind.{attribute.Type} 타입인데, " +
                    $"SoundKind.{expectedKind}로 재생하려고 했습니다.");
                return false;
            }

            return true;
        }

        private void PlaySFXClip(AudioClip clip)
        {
            if (_sfxSource == null)
            {
                Debug.LogError("SFX AudioSource가 할당되지 않았습니다.", this);
                return;
            }

            _sfxSource.PlayOneShot(clip);
        }

        private void PlayBGMClip(AudioClip clip)
        {
            if (_bgmSource == null)
            {
                Debug.LogError("BGM AudioSource가 할당되지 않았습니다.", this);
                return;
            }

            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
                return;

            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }
    }
}