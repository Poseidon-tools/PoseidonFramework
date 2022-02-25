using DG.Tweening;
using UnityEngine;

namespace Utils
{
    public class RotateTween : MonoBehaviour
    {
        [SerializeField] private Vector3 axis = new Vector3(0, 0, 360);
        [SerializeField] private float duration = 2.0f;
        [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private int loops = -1;
        [SerializeField] private bool isRelative = true;

        private Tween currentTween;

        private void OnEnable()
        {
            currentTween = transform
                           .DOLocalRotate(axis, duration)
                           .SetEase(ease)
                           .SetLoops(loops);
            if(isRelative)
                currentTween.SetRelative();
        }

        private void OnDisable()
        {
            currentTween.Kill();
        }
    }
}
