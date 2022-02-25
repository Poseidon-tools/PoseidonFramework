namespace Core.Input
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    public interface IInputController
    {
        void Initialize(PlayerInput playerInput);
        void Dispose();
        
        Vector2 GetPrimaryTouchPosition();

        event Action<Vector2, float> OnPrimaryTouchStarted;
        event Action<Vector2, float> OnPrimaryTouchEnded;
    }
}