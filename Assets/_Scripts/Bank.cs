using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace BlackHole {
    [CreateAssetMenu(fileName = "Bank", menuName = "ScriptableObject/Bank")]
    public class Bank : ScriptableObject
    {

        public int AvailableCurrency = 3000;
        public static event Action<int> OnCashTransfer;

        public void CashTransfer(int cash)
        {
            if (AvailableCurrency + cash < 0)
            {
                Debug.Log("Account cannot go negative");
                return;
            }

            AvailableCurrency += cash;
            OnCashTransfer?.Invoke(cash);
        }
    }
}