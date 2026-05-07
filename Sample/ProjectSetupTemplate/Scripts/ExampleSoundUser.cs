using LSH.Core;
using UnityEngine;

namespace NewGame
{
    public class ExampleSoundUser : MonoBehaviour
    {
        [SerializeField, SoundIdField(SoundType.SFX)]
        private SoundId _clickSound;

        public void PlayByInspectorId()
        {
            SoundManager.Instance.PlaySFX(_clickSound);
        }

        public void PlayByEnum()
        {
            SoundManager.Instance.PlaySFX(SFXID.UI_Btn_Click);
        }
    }
}