namespace Core.Controller.Drag
{
    using System;
    using Input;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class DragController : MonoBehaviour
    {
        #region Public Events
        public event Action<Vector2> OnDragStarted;
        /// <summary>
        /// Event invoked when drag ends.
        /// Sending drag end position and drag DURATION.
        /// </summary>
        public event Action<Vector2, float> OnDragEnded;
        public event Action<Vector2> OnDragUpdated;
        #endregion
        #region Inspector
        [SerializeField] private PlayerInput playerInput;
        [SerializeField, BoxGroup("Settings")] private float minDistanceToSendDragUpdate = 2f;
        #endregion
        #region Private Variables
        private IInputController inputController;
        private float touchStartTime;
        private bool isEnabled;
        private Vector2 lastFrameDragPosition;
        
        [ShowInInspector] private bool touchStarted;
        #endregion
        #region Public API
        public void Enable()
        {
            isEnabled = true;
        }
        public void Disable()
        {
            isEnabled = false;
        }
        #endregion
        #region Unity Methods
        private void Awake()
        {
            inputController = new InputController();
            inputController.Initialize(playerInput);
        }
        private void OnEnable()
        {
            RegisterListeners();
        }
        private void OnDisable()
        {
            UnregisterListeners();
        }

        private void Update()
        {
            if (!isEnabled)
            {
                return;
            }

            if (!touchStarted) return;
            var currentPos = inputController.GetPrimaryTouchPosition();
            if (!CheckForMinDragDistance(currentPos)) return;
            OnDragUpdated?.Invoke(currentPos);
            lastFrameDragPosition = currentPos;
        }
        #endregion
        #region Private Methods
        private void RegisterListeners()
        {
            inputController.OnPrimaryTouchStarted += OnTouchStartedHandler;
            inputController.OnPrimaryTouchEnded += OnTouchEndedHandler;
        }
        private void UnregisterListeners()
        {
            inputController.OnPrimaryTouchStarted -= OnTouchStartedHandler;
            inputController.OnPrimaryTouchEnded -= OnTouchEndedHandler;
        }
        private void OnTouchEndedHandler(Vector2 position, float timeStamp)
        {
            if (!isEnabled)
            {
                return;
            }
            touchStarted = false;
            OnDragEnded?.Invoke(position, touchStartTime - timeStamp);
        }
        private void OnTouchStartedHandler(Vector2 position, float timeStamp)
        {
            if (!isEnabled)
            {
                return;
            }
            touchStartTime = timeStamp;
            touchStarted = true;
            lastFrameDragPosition = position;
            OnDragStarted?.Invoke(position);
        }

        private bool CheckForMinDragDistance(Vector2 currentPos)
        {
            return Mathf.Abs(Vector3.Distance(currentPos, lastFrameDragPosition)) > minDistanceToSendDragUpdate;
        }
        #endregion
    }
}