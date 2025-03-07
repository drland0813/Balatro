using TMPro;
using UnityEngine;

namespace GamePlay
{
    public class ChipScoreUI : MonoBehaviour{
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void Init(int score)
        {
            _scoreText.text = $"+{score.ToString()}";
            Show(true);
        }
        public void Show(bool enable)
        {
            gameObject.SetActive(enable);
        }
    }
}