using System;
using UnityEngine;
namespace GamePlay
{
    public class GamePlayController : MonoBehaviour
    {
        public Action OnWin;
        public Action OnLose;

        public void BackToMainMenu()
        {
            OnLose?.Invoke();
        }
    }
}
