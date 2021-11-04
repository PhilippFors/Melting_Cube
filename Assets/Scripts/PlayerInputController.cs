using UsefulCode.Input;
using UsefulCode.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Player.PlayerInput
{
    /// <summary>
    /// Contains all of the inputs the player can use.
    /// Class is singleton so other classes can easily access input actions.
    /// </summary>
    public class PlayerInputController : SingletonBehaviour<PlayerInputController>
    {
        public InputActionData<Vector2> MousePosition => mousePosition ?? (mousePosition = new InputActionData<Vector2>(mousePositionAction));
        public InputActionData<Vector2> MouseDelta => mouseDelta ?? (mouseDelta = new InputActionData<Vector2>(mouseDeltaAction));
        public InputActionData<float> LeftMouseButton => leftMouseButton ?? (leftMouseButton = new InputActionData<float>(leftMouseButtonAction));
        public InputActionData<float> RightMouseButton => rightMouseButton ?? (rightMouseButton = new InputActionData<float>(rightMouseButtonAction));
        public InputActionData<float> Pause => pause ?? (pause = new InputActionData<float>(pauseAction));

        [SerializeField] private InputActionAsset inputActions;

        [SerializeField] private InputActionProperty mousePositionAction;
        [SerializeField] private InputActionProperty mouseDeltaAction;
        [SerializeField] private InputActionProperty leftMouseButtonAction;
        [SerializeField] private InputActionProperty rightMouseButtonAction;
        [SerializeField] private InputActionProperty pauseAction;

        private InputActionData<Vector2> mousePosition;
        private InputActionData<Vector2> mouseDelta;
        private InputActionData<float> leftMouseButton;
        private InputActionData<float> rightMouseButton;
        private InputActionData<float> pause;

        private void OnEnable()
        {
            EnableControls();
        }

        private void OnDisable()
        {
            DisableControls();
        }

        public void EnableControls()
        {
            foreach (var action in inputActions)
            {
                action?.Enable();
            }
        }

        public void DisableControls()
        {
            foreach (var action in inputActions)
            {
                action?.Disable();
            }
        }
    }
}