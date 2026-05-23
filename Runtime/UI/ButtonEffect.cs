using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSH.Core
{
    [RequireComponent(typeof(Button))]
    public class ButtonEffect : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [Header("Sound")]
        [SerializeField, SoundIdField(SoundType.SFX)] private SoundId _clickSoundId;
        [SerializeField, SoundIdField(SoundType.SFX)] private SoundId _hoverSoundId;


        [Header("Motion")]
        [SerializeField] private float _hoverScale = 1.03f;
        [SerializeField] private float _pressedScale = 0.96f;
        [SerializeField] private Vector2 _pressedOffset = new(0f, -3f);

        private Button _button;
        private RectTransform _rectTransform;
        private Vector3 _originScale;
        private Vector2 _originPosition;
        private bool _isHovering;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _rectTransform = transform as RectTransform;
            _originScale = _rectTransform.localScale;
            _originPosition = _rectTransform.anchoredPosition;

            _button.onClick.AddListener(PlayClickSound);

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            SoundManager.Instance.PlaySFX(_hoverSoundId);

            _rectTransform.localScale = _originScale * _hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;

            _rectTransform.localScale = _originScale;
            _rectTransform.anchoredPosition = _originPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _rectTransform.localScale = _originScale * _pressedScale;
            _rectTransform.anchoredPosition = _originPosition + _pressedOffset;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _rectTransform.localScale = _isHovering
                ? _originScale * _hoverScale
                : _originScale;

            _rectTransform.anchoredPosition = _originPosition;
        }

        private void PlayClickSound()
        {
            SoundManager.Instance.PlaySFX(_clickSoundId);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(PlayClickSound);
        }
    }
}