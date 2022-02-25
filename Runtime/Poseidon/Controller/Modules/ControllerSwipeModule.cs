namespace Core.Controller.Modules
{
    using System;
    using Input;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class ControllerSwipeModule : MonoBehaviour
    {
         public event Action<SwipeData> OnSwipe;
        #region Swipe Inspector
        [BoxGroup("Swipe")] [SerializeField] private bool detectSwipeOnlyAfterRelease;

        [BoxGroup("Swipe")] [SerializeField] private float minDistanceForSwipe = 20f;
        
        #endregion
        #region Private Variables
        private Vector2 fingerDownPosition;
        private Vector2 fingerUpPosition;

        private IInputController inputController;
        private bool isTouchActive;

        private bool isEnabled;

        #endregion

        #region Public Methods
        public void Initialize(IInputController inputController)
        {
            this.inputController = inputController;
            
            inputController.OnPrimaryTouchStarted += OnPrimaryTouchStartedHandler;
            inputController.OnPrimaryTouchEnded += OnPrimaryTouchEndedHandler;
        }

        public void Enable(bool enable)
        {
            isEnabled = enable;
        }
        #endregion
        
        #region Swipe
        
        private void DetectSwipe()
        {
            if (!SwipeDistanceCheckMet())
            {
                return;
            }
            
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0
                    ? SwipeDirection.Up
                    : SwipeDirection.Down;
                SendSwipe(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0
                    ? SwipeDirection.Right
                    : SwipeDirection.Left;
                SendSwipe(direction);
            }

            fingerUpPosition = fingerDownPosition;
        }
        private bool IsVerticalSwipe()
        {
            return VerticalMovementDistance() > HorizontalMovementDistance();
        }
        private bool SwipeDistanceCheckMet()
        {
            return VerticalMovementDistance() > minDistanceForSwipe ||
                   HorizontalMovementDistance() > minDistanceForSwipe;
        }
        private float VerticalMovementDistance()
        {
            return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
        }
        private float HorizontalMovementDistance()
        {
            return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
        }
        private void SendSwipe(SwipeDirection direction)
        {
            if (!isEnabled)
            {
                return;
            }
            var swipeData = new SwipeData()
                            {
                                Direction = direction,
                                StartPosition = fingerDownPosition,
                                EndPosition = fingerUpPosition
                            };
            OnSwipe?.Invoke(swipeData);
        }
        #endregion
        #region Unity Methods
        private void OnDisable()
        {
            inputController.OnPrimaryTouchStarted -= OnPrimaryTouchStartedHandler;
            inputController.OnPrimaryTouchEnded -= OnPrimaryTouchEndedHandler;
        }
        private void OnPrimaryTouchStartedHandler(Vector2 touchPosition, float time)
        {
            fingerUpPosition = touchPosition;
            fingerDownPosition = touchPosition;

            isTouchActive = true;
        }
        
        private void OnPrimaryTouchEndedHandler(Vector2 touchPosition, float time)
        {
            fingerDownPosition = touchPosition;
            DetectSwipe();
            
            isTouchActive = false;
        }
        

        private void Update()
        {
            if(detectSwipeOnlyAfterRelease || !isTouchActive)
            {
                return;
            }

            fingerDownPosition = inputController.GetPrimaryTouchPosition();
            DetectSwipe();
        }
        #endregion
    }
}
