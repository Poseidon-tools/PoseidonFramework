namespace Core.Controller
{
    using System;
    using Input;
    using Modules;
    using UnityEngine;

    public class ControllerManager : MonoBehaviour, IControllerManager
    {
        public event Action<SwipeData> OnSwipe
        {
            add => swipeModule.OnSwipe += value;
            remove => swipeModule.OnSwipe -= value;
        }
        
        [SerializeField] private ControllerSwipeModule swipeModule;

        public void Initialize(IInputController inputController)
        {
            swipeModule.Initialize(inputController);
        }

        public void Enable()
        {
            swipeModule.Enable(true);
        }

        public void Disable()
        {
            swipeModule.Enable(false);
        }
    }

}