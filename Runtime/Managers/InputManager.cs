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
            _actionAsset.Enable();
        }

        public void SetActionMap(string mapName)
        {
            _currentMap?.Disable();
            _currentMap = _actionAsset.FindActionMap(mapName);
            _currentMap?.Enable();
        }

        public InputAction GetAction(string actionName)
            => _currentMap?.FindAction(actionName);

        protected override void OnDestroy()
        {
            _actionAsset.Disable();
            base.OnDestroy();
        }
    }
}
