using UnityEngine;

namespace GamePlay
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private ScoreBoardView _view;

        private int _score
        {
            set => _view.UpdateScore(value);
        }

        public void UpdateScore()
        {
            _score = 1000;
        }

    }
}