using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Balatro
{
    public class MoneyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;

        private void Start()
        {
            MoneyController.GetInstance().OnMoneyChanged += UpdateMoneyText;
            UpdateMoneyText(MoneyController.GetInstance().GetMoney()); 
        }

        private void OnDestroy()
        {
            MoneyController.GetInstance().OnMoneyChanged -= UpdateMoneyText;
        }

        private void UpdateMoneyText(int currentMoney)
        {
            moneyText.text = $"${currentMoney}";
        }
    }
}
