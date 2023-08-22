using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DamageNumbersPro;

namespace BlackHole {
    public class Bank : MonoBehaviour
    {
        public static Bank Instance;
        public int AvailableCurrency = 3000;
        private TMP_Text _currencyText;

        [SerializeField] private DamageNumber cashNumberPosPrefab;
        [SerializeField] private DamageNumber cashNumberNegPrefab;
        [SerializeField] private RectTransform cashNumberRectParent;
        private DamageNumber cashNumber;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            AvailableCurrency = ES3.Load<int>("Cash", 0);
            _currencyText = GetComponent<TMP_Text>();
            _currencyText.text = "$" + AvailableCurrency.ToString();
        }

        public void CashTransfer(int cash)
        {
            if (AvailableCurrency + cash < 0)
            {
                Debug.Log("Account cannot go negative");
                return;
            }

            cashNumber = cash >= 0 ? cashNumberPosPrefab.Spawn(Vector3.zero, cash) : cashNumberNegPrefab.Spawn(Vector3.zero, -cash);
            cashNumber.SetAnchoredPosition(cashNumberRectParent, new Vector2(0, 0));

            AvailableCurrency += cash;
            _currencyText.text = "$" + AvailableCurrency.ToString();
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Kaching);
            ES3.Save("Cash", AvailableCurrency);
        }
    }
}