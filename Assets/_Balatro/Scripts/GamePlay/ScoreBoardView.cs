using TMPro;
using UnityEngine;

namespace GamePlay
{
    public class ScoreBoardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void UpdateScore(int score)
        {
            _scoreText.text += score.ToString();
        }
    }
}