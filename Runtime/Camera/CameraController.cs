using System.Collections;
using UnityEngine;

namespace LSH.Core
{
    public enum CameraFocusAxis
    {
        XY,
        X,
        Y
    }
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [Header("Focus")]
        [SerializeField] private Vector3 _offset = new(0f, 0f, -10f);
        [SerializeField] private float _moveDuration = 0.25f;
        [SerializeField] private CameraFocusAxis _focusAxis = CameraFocusAxis.XY;

        [Header("Zoom")]
        [SerializeField] private float _zoomSpeed = 0.5f;
        [SerializeField] private float _minZoom = 5f;
        [SerializeField] private float _maxZoom = 8f;

        private Coroutine _moveCoroutine;

        public void FocusOn(Vector3 targetPosition)
        {
            if (_camera == null)
                return;

            Vector3 destination = GetFocusDestination(targetPosition);

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            _moveCoroutine = StartCoroutine(MoveTo(destination));
        }

        private Vector3 GetFocusDestination(Vector3 targetPosition)
        {
            Vector3 currentPosition = _camera.transform.position;

            float x = _focusAxis is CameraFocusAxis.X or CameraFocusAxis.XY
                ? targetPosition.x + _offset.x
                : currentPosition.x;

            float y = _focusAxis is CameraFocusAxis.Y or CameraFocusAxis.XY
                ? targetPosition.y + _offset.y
                : currentPosition.y;

            return new Vector3(x, y, _offset.z);
        }

        public void Zoom(float scrollValue)
        {
            if (_camera == null)
                return;

            if (Mathf.Abs(scrollValue) <= 0.01f)
                return;

            float nextSize = _camera.orthographicSize - scrollValue * _zoomSpeed;
            _camera.orthographicSize = Mathf.Clamp(nextSize, _minZoom, _maxZoom);
        }

        private IEnumerator MoveTo(Vector3 destination)
        {
            Vector3 start = _camera.transform.position;
            float timer = 0f;

            while (timer < _moveDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / _moveDuration);

                _camera.transform.position = Vector3.Lerp(start, destination, t);
                yield return null;
            }

            _camera.transform.position = destination;
            _moveCoroutine = null;
        }
    }
}