using Gameplay.Player;
using Core.Shared;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Gameplay.Player.PlayerInput;

namespace Gameplay.Map
{
    /// <summary>
    /// Controls the opening and closing of the map UI.
    /// </summary>
    public class MapToggle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _mapUI;

        private PlayerInput _playerInput;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="playerInput">Injected player input system.</param>
        public void Initialize(PlayerInput playerInput)
        {
            _playerInput = playerInput;

            // safeguard
            if (_playerInput == null)
            {
                Debug.LogError("MapToggle: PlayerInput reference is NULL. MapKey will not work.");
                return;
            }
            _mapUI.SetActive(false);
            // subscribe to event
            _playerInput.MapToggle.performed += OnMapKey;
        }


        private void OnDestroy()
        {
            // cleanup
            if (_playerInput != null && _playerInput.MapToggle != null)
                _playerInput.MapToggle.performed -= OnMapKey;
        }


        /// <summary>
        /// Called whenever player presses the map key.
        /// </summary>
        private void OnMapKey(InputAction.CallbackContext ctx)
        {
            _mapUI.SetActive(!_mapUI.activeSelf);
            Debug.Log("MapToggle: Toggled map UI to " + _mapUI.activeSelf);
        }
    }
}
