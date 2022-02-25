namespace Core.Controller
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class SwipeArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Public Variables
        public event Action<SwipeData> OnSwipeInAreaDetected;
        #endregion

        #region Inspector
        [SerializeField] private List<SwipeDirection> listenedDirections;
        #endregion
        #region Private Variables
        private IControllerManager controllerManager;
        private bool listenForSwipeEvents;
        private RectTransform rectTransform;
        private bool hasPutFinger;

        #endregion
        #region Public Methods
        public void Initialize(IControllerManager inControllerManager)
        {
            this.controllerManager = inControllerManager;
            this.controllerManager.OnSwipe += OnSwipeDetected;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (hasPutFinger || controllerManager == null) return;
            hasPutFinger = true;
            controllerManager.OnSwipe += OnSwipeDetected;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (controllerManager == null) return;
            hasPutFinger = false;
            controllerManager.OnSwipe -= OnSwipeDetected;
        }

        public void Dispose()
        {
            if(controllerManager != null)
            {
                controllerManager.OnSwipe -= OnSwipeDetected;
            }
            controllerManager = null;
        }

        #endregion
        
        #region Private Methods
        private void OnSwipeDetected(SwipeData swipeData)
        {
            if(!listenedDirections.Contains(swipeData.Direction))
            {
                return;
            }
            OnSwipeInAreaDetected?.Invoke(swipeData);
        }
        #endregion
    }
}