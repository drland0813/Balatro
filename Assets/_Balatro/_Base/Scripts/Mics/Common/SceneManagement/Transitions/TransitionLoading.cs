using UnityEngine;

namespace Common
{
    public class TransitionLoading : SceneTransitionSequential
    {
        public float minDuration;
        public System.Action OnFinishedLoading;

        private System.DateTime _timeFinish;
        private bool _isFinishLoading;

        public void LoadingFinished()
        {
            _isFinishLoading = true;
        }

        public override void Enter()
        {
            base.Enter();
            _timeFinish = System.DateTime.UtcNow.AddSeconds(minDuration);
        }

        public override bool StepIn(float dt)
        {
            if (_isFinishLoading && System.DateTime.UtcNow > _timeFinish)
            {
                OnFinishedLoading?.Invoke();
                return true;
            }

            return false;
        }
    }
}

