namespace Core.Input
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputController : IInputController
    {
        #region Public Variables
        public event Action<Vector2, float> OnPrimaryTouchStarted;
        public event Action<Vector2, float> OnPrimaryTouchEnded;
        #endregion
        
        #region Private Variables
        private InputActions inputActions;
        private PlayerInput playerInput;
        #endregion

        #region Public Methods
        Vector2 IInputController.GetPrimaryTouchPosition()
        {
            return inputActions.BaseInput.PrimaryPosition.ReadValue<Vector2>();
        }

        void IInputController.Initialize(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
            inputActions = new InputActions();
            inputActions.Enable();
            RegisterCallbacks();
        }

        void IInputController.Dispose()
        {
            inputActions.Disable();
            UnregisterCallbacks();
        }
        #endregion

        #region Private Methods
        
        private void RegisterCallbacks()
        {
            inputActions.BaseInput.PrimaryContact.started += OnPrimaryContactStarted;
            inputActions.BaseInput.PrimaryContact.canceled += OnPrimaryContactCanceled;
        }

        private void UnregisterCallbacks()
        {
            inputActions.BaseInput.PrimaryContact.started -= OnPrimaryContactStarted;
            inputActions.BaseInput.PrimaryContact.canceled -= OnPrimaryContactCanceled;
        }
        #endregion

        #region Input Methods
        
        private void OnPrimaryContactCanceled(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.canceled)
            {
                OnPrimaryTouchEnded?.Invoke(inputActions.BaseInput.PrimaryPosition.ReadValue<Vector2>(),
                    (float) callbackContext.time);
            }
        }
        
        private void OnPrimaryContactStarted(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.started)
            {
                OnPrimaryTouchStarted?.Invoke(inputActions.BaseInput.PrimaryPosition.ReadValue<Vector2>(),
                    (float) callbackContext.startTime);
            }
        }
        #endregion
        
    }
}