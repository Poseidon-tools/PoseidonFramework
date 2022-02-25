namespace Core.ViewManager.Animation
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    public class SimpleHideAnimation : AnimationSequence
    {
        [SerializeField] private RectTransform rectTransform;

        private Ease ease = Ease.OutQuint;
        private float animationTime = 0.3f;

        private Vector2 cacheAnchorPosition = Vector2.zero;

        public override void Play(Action onComplete)
        {
            //  Reset rect position
            cacheAnchorPosition = rectTransform.anchoredPosition;
            cacheAnchorPosition.x = 0;

            rectTransform.anchoredPosition = cacheAnchorPosition;

            sequence?.Kill();
            sequence = DOTween.Sequence()
                .Append(rectTransform.DOAnchorPosX(-rectTransform.rect.width, animationTime)
                .OnComplete(() => onComplete?.Invoke())
                .SetEase(ease));

            sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
            sequence.Play();
        }
    }
}