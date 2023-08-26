using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace BlackHole {
    [CreateAssetMenu(fileName = "Bank", menuName = "ScriptableObject/Bank")]
    public class Bank : ScriptableObject
    {

        public int AvailableCurrency { get; private set; } = 0;

        public static event Action<int> OnCashTransfer;

        public void CashTransfer(int cash)
        {
            if (AvailableCurrency + cash < 0)
            {
                Debug.Log("Account cannot go negative");
                return;
            }

            AvailableCurrency += cash;
            AvailableCurrency = Mathf.Clamp(AvailableCurrency, 0, 99999);
            OnCashTransfer?.Invoke(cash);
        }

        public void FileForBankcruptcy()
        {
            // Use this rather than setting directly to 0 in order to invoke event
            CashTransfer(-AvailableCurrency);
        }
    }
}