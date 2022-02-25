namespace Core.Controller
{
    using System;
    using Input;

    public interface IControllerManager
    {
        event Action<SwipeData> OnSwipe;
        
        void Enable();
        void Disable();
        void Initialize(IInputController inputController);
    }
}