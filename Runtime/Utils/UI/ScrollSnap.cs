namespace Utils.UI
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;
    
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollSnap<T, U> : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler 
        where T : MonoBehaviour, ICell<U> where U : ICellData
    {
        public event Action<U> OnItemSelected;
        public event Action<U> OnItemChanging;

        #region Fields

        [SerializeField] private T itemPrefab;
        [SerializeField] private MovementAxis movementAxis = MovementAxis.Horizontal;
        
        [SerializeField] private bool swipeGestures = true;
        [SerializeField] private float minimumSwipeSpeed = 0f;
        [SerializeField] private SnapTarget snapTarget = SnapTarget.Next;
        [SerializeField, Range(0, 50f)] private float snappingSpeed = 10f;
        [SerializeField] private float thresholdSnappingSpeed = -1f;
        [SerializeField] private bool hardSnap = true;
        [SerializeField] private bool useUnscaledTime = false;
        
        [SerializeField] private bool useCustomWidth = false;
        [SerializeField] private bool useCustomHeight = false;
        [SerializeField] private float customWidth = 0f;
        [SerializeField] private float customHeight = 0f;
        [SerializeField] private float spacing = 0f;

        private Vector2 itemCellSize = Vector2.zero;
        private bool dragging, selected = true, pressing;
        private float releaseSpeed, contentLength;
        private Direction releaseDirection;
        private Canvas canvas;
        private ScrollRect scrollRect;
        private RectTransform viewportRectTransform;
        private RectTransform contentRectTransform;

        private List<ItemData<T>> items = new List<ItemData<T>>();
        private List<U> itemsData;
       
        
        #endregion

        #region Properties
        
        public int CurrentItem { get; set; }
        public int TargetItemIndex { get; set; }

        public int ItemsCount => items.Count;

        public ItemData<T> NearestItemData
        {
            get => nearestItemData;
            set
            {
                if(nearestItemData == value) return;
                nearestItemData = value;
                nearestItemDataIndex = items.IndexOf(nearestItemData);
                OnItemChanging?.Invoke(itemsData[nearestItemDataIndex]);
            }
        }

        private ItemData<T> nearestItemData;

        private int nearestItemDataIndex;

        private bool ContentIsUpdated() => contentRectTransform.rect.height > 0;
        #endregion

        #region Enumerators
        public enum MovementAxis
        {
            Horizontal,
            Vertical,
        }
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }
        public enum SnapTarget
        {
            Nearest,
            Previous,
            Next,
        }
        #endregion

        #region Methods
        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            canvas = GetComponentInParent<Canvas>();
            viewportRectTransform = scrollRect.viewport;
            contentRectTransform = scrollRect.content;
         
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
            scrollRect.vertical = (movementAxis == MovementAxis.Vertical);
            scrollRect.horizontal = (movementAxis == MovementAxis.Horizontal);
        }

        private void Update()
        {
            if(ItemsCount == 0) return;
            
            OnSelectingAndSnapping();
            CalculateItemsDistanceFromCenter();
        }

        /// <summary>
        /// Calculates the distance for every item and sets the nearest one.
        /// </summary>
        private void CalculateItemsDistanceFromCenter()
        {
            float distance = float.MaxValue;
            var nearestItem = NearestItemData;

            for(int i = 0; i < itemsData.Count; i++)
            {
                // calculate the distance from center on each item
                float currentDistance = items[i].CalculateDistanceFromCenter(viewportRectTransform).magnitude;
                
                if(currentDistance < distance)
                {
                    // if distance is smaller, catch nearest item
                    distance = items[i].DistanceFromCenter.magnitude;
                    nearestItem = items[i];
                }
            }
            
            // assign nearest item
            NearestItemData = nearestItem;
        }

        public void SetData(List<U> itemsData, int startIndex = -1)
        {
            this.itemsData = itemsData;

            var rect = viewportRectTransform.rect;
            itemCellSize.x = useCustomWidth ? customWidth : rect.size.x;
            itemCellSize.y = useCustomHeight ? customHeight : rect.size.y;
            
            // add new items if necessary
            if(items.Count < itemsData.Count)
            {
                for(int i = items.Count; i < itemsData.Count; i++)
                {
                    var instantiatedItem = Instantiate(itemPrefab, contentRectTransform);
                    var item = new ItemData<T>(instantiatedItem);
                    items.Add(item);
                }
            }
            
            // Update all the cells with data
            for(int i = 0; i < items.Count; i++)
            {
                SetPositionForItem(i);
                
                if(i >= itemsData.Count)
                {
                    items[i].Item.gameObject.SetActive(false);
                    continue;
                }
                items[i].Item.gameObject.SetActive(true);
                items[i].Item.UpdateCell(itemsData[i]);
            }
            
            TargetItemIndex = startIndex > -1 ? startIndex : 0;
            SnapToTargetItem(true);
        }

        private void SetPositionForItem(int itemIndex)
        {
            Vector2 position;
            position.x = movementAxis == MovementAxis.Vertical ? 0f : itemIndex * itemCellSize.x + itemIndex * spacing;
            position.y = movementAxis == MovementAxis.Horizontal? 0f : itemIndex * -itemCellSize.y - itemIndex * spacing;

            var center = new Vector2(0.5f, 0.5f);
            items[itemIndex].ItemRectTransform.pivot = center;
            items[itemIndex].ItemRectTransform.anchorMin = center;
            items[itemIndex].ItemRectTransform.anchorMax = center;
            items[itemIndex].ItemRectTransform.anchoredPosition = position;
            items[itemIndex].ItemRectTransform.sizeDelta = itemCellSize;
        }
        
        private void SnapToTargetItem(bool immediate = false)
        {
            var position = -items[TargetItemIndex].ItemRectTransform.anchoredPosition;
            contentRectTransform.anchoredPosition = immediate ? position : Vector2.Lerp(contentRectTransform.anchoredPosition, position, (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * snappingSpeed);

            if(CurrentItem == TargetItemIndex || !(items[TargetItemIndex].DistanceFromCenter.magnitude < 10f)) return;

            CurrentItem = TargetItemIndex;
            OnItemSelected?.Invoke(itemsData[CurrentItem]);
        }
        
        private void SelectTargetItem()
        {

            Vector2 distanceFromCenter = nearestItemData.DistanceFromCenter;

            if (snapTarget == SnapTarget.Nearest || releaseSpeed <= minimumSwipeSpeed)
            {
                GoToItem(nearestItemDataIndex);
            }
            else if (snapTarget == SnapTarget.Previous)
            {
                if ((releaseDirection == Direction.Right && distanceFromCenter.x < 0f) || (releaseDirection == Direction.Up && distanceFromCenter.y < 0f))
                {
                    // next item
                    GoToItem( Math.Min(nearestItemDataIndex + 1, itemsData.Count));
                }
                else if ((releaseDirection == Direction.Left && distanceFromCenter.x > 0f) || (releaseDirection == Direction.Down && distanceFromCenter.y > 0f))
                {
                    // previous item
                    GoToItem( Math.Max(0, nearestItemDataIndex - 1));
                }
                else
                {
                    GoToItem(nearestItemDataIndex);
                }
            }
            else if (snapTarget == SnapTarget.Next)
            {
                if ((releaseDirection == Direction.Right && distanceFromCenter.x > 0f) || (releaseDirection == Direction.Up && distanceFromCenter.y > 0f))
                {
                    // previous item
                    GoToItem( Math.Max(0, nearestItemDataIndex - 1));
                }
                else if ((releaseDirection == Direction.Left && distanceFromCenter.x < 0f) || (releaseDirection == Direction.Down && distanceFromCenter.y < 0f))
                {
                    // next item
                    GoToItem( Math.Min(nearestItemDataIndex + 1, itemsData.Count));
                }
                else
                {
                    GoToItem(nearestItemDataIndex);
                }
            }
        }
        
        

        private void OnSelectingAndSnapping()
        {
            if (selected)
            {
                if (!((dragging || pressing) && swipeGestures))
                {
                    SnapToTargetItem();
                }
            }
            else if (!dragging && (scrollRect.velocity.magnitude <= thresholdSnappingSpeed || thresholdSnappingSpeed == -1f))
            {
                SelectTargetItem();
            }
        }

        public void GoToItem(int itemIndex)
        {
            TargetItemIndex = itemIndex;
            selected = true;

            if (hardSnap)
            {
                scrollRect.inertia = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData) => pressing = true;

        public void OnPointerUp(PointerEventData eventData) => pressing = false;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (hardSnap)
            {
                scrollRect.inertia = true;
            }

            selected = false;
            dragging = true;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;

            releaseDirection = movementAxis switch
                               {
                                   MovementAxis.Horizontal => scrollRect.velocity.x > 0 ? Direction.Right : Direction.Left,
                                   MovementAxis.Vertical => scrollRect.velocity.y > 0 ? Direction.Up : Direction.Down,
                                   _ => releaseDirection,
                               };

            releaseSpeed = scrollRect.velocity.magnitude;
        }
        
        #endregion
    }

    public class ItemData<T> where T : MonoBehaviour
    {
        public RectTransform ItemRectTransform { get; }
        public T Item { get; }
        public Vector2 DistanceFromCenter => distanceFromCenter;
        private Vector2 distanceFromCenter;

        public ItemData(T item)
        {
            Item = item;
            ItemRectTransform = item.transform as RectTransform;
        }

        public Vector2 CalculateDistanceFromCenter(RectTransform viewportRectTransform)
        {
            var itemPosition = ItemRectTransform.position;
            var viewportPosition = viewportRectTransform.position;
            distanceFromCenter.x = Mathf.Abs(itemPosition.x - viewportPosition.x);
            distanceFromCenter.y = Mathf.Abs(itemPosition.y - viewportPosition.y);
            return distanceFromCenter;
        }
    }
}
