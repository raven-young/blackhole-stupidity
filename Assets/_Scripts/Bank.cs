using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace BlackHole {
    [CreateAssetMenu(fileName = "Bank", menuName = "ScriptableObject/Bank")]
    public class Bank : ScriptableObject
    {

        // The Singleton instance
        private static Bank _instance;

        // Property to access the Singleton instance
        public static Bank Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (!ES3.KeyExists("Bank"))
                    {

                        _instance = Resources.Load<Bank>("_ScriptableObjects/Bank");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (_instance == null)
                        {
                            _instance = CreateInstance<Bank>();
                        }
                        ES3.Save("Bank", _instance);
                        Debug.Log("Saved non-existent Bank key: " + _instance);
                    }
                    else
                    {
                        _instance = ES3.Load<Bank>("Bank");
                        Debug.Log("Loaded Bank asset: " + _instance);
                    }
                }
                return _instance;
            }

            set => _instance = value;
        }

        public static int AvailableCurrency { get; set; }

        public static event Action<int> OnCashTransfer;

        public static void CashTransfer(int cash)
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