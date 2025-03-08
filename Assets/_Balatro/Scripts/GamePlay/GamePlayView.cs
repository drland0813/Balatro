using Common.UI;
using UnityEngine;

namespace GamePlay
{
    public class GamePlayView : UIController
    {

        public GameObject _runInfoView;

        public void EnableRunInfoUI()
        {
            _runInfoView.SetActive(true);
        }

        public void DisableRunInfoUI()
        {
            _runInfoView.SetActive(false);
        }
    }
}