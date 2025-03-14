using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public class MoneyController : MonoBehaviour
    {
        public static MoneyController Instance { get; private set; }
        
        private int _money = 0;
        public event Action<int> OnMoneyChanged;
        
        public static MoneyController GetInstance()
        {
            if (Instance == null)
            {
                GameObject obj = new GameObject("MoneyController");
                Instance = obj.AddComponent<MoneyController>();

                DontDestroyOnLoad(obj);
            }
            return Instance;
        }
        
        public void AddMoney(int amount)
        {
            _money += amount;
            OnMoneyChanged?.Invoke(_money);
        }
        
        public void SubtractMoney(int amount)
        {
            _money = Mathf.Max(0, _money - amount);
            OnMoneyChanged?.Invoke(_money);
        }
        
        public void SetMoney(int amount)
        {
            _money = Mathf.Max(0, amount);
            OnMoneyChanged?.Invoke(_money);
        }
        
        public int GetMoney()
        {
            return _money;
        }
    }
}
