namespace Core.ViewManager
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    public abstract class AnimationSequence : MonoBehaviour
    {
        protected Sequence sequence;

        public abstract void Play(Action onComplete);

        public virtual void PlayInstantly(){}

        [ContextMenu("Stop")]
        public virtual void Stop()
        {
            if (sequence == null || !sequence.IsPlaying()) return;
            sequence.Pause();
            sequence.Kill();
        }

        public void Complete()
        {
            sequence.Complete();
        }
    }
}
