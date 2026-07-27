using UnityEngine;
using UnityEngine.InputSystem;

namespace LSH.Core
{
    public class InputManager : Singleton<InputManager>, IBootable
    {
        [SerializeField] private InputActionAsset _actionAsset;
        private InputActionMap _currentMap;

        public void Init()
        {
            if (_actionAsset == null)
            {
                Debug.LogError("InputActionAsset is not assigned.", this);
                return;
            }

            _actionAsset.Enable();
        }

        public void SetActionMap(string mapName)
        {
            if (_actionAsset == null)
            {
                Debug.LogError("InputActionAsset is not assigned.", this);
                return;
            }

            _currentMap?.Disable();
            _currentMap = _actionAsset.FindActionMap(mapName);

            if (_currentMap == null)
            {
                Debug.LogWarning($"Input action map was not found: {mapName}", this);
                return;
            }

            _currentMap.Enable();
        }

        public InputAction GetAction(string actionName)
            => _currentMap?.FindAction(actionName);

        protected override void OnDestroy()
        {
            if (_actionAsset != null)
                _actionAsset.Disable();

            base.OnDestroy();
        }
    }
}
