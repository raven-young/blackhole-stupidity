using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace BlackHole
{
    public class UpgradeSlot : MonoBehaviour
    {
        public UpgradeManager.Upgrade ActiveUpgrade;
        public GameObject ActiveUpgradeButton;

        [SerializeField] private GameObject _buyPanel;
        private TMP_Text _slotText;
        [SerializeField] private int _unlockCost;
        public bool Unlocked;
        private bool _isBuying = false;

        private void Awake()
        {
            _slotText = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (ActiveUpgrade == null || ActiveUpgrade.Name == "")
            {
                ResetSlot();
            }

            if (!Unlocked)
            {
                transform.Find("CostText").GetComponent<TMP_Text>().text = "$" + _unlockCost;
            }
        }

        private void OnEnable()
        {
            UpgradeListDisplay.OnUpgradeEquipped += Equip;
            UpgradeListDisplay.OnUpgradeUnequipped += Unequip;
            Button.BuyComplete += FinishBuy;
        }

        private void OnDisable()
        {
            UpgradeListDisplay.OnUpgradeEquipped -= Equip;
            UpgradeListDisplay.OnUpgradeUnequipped -= Unequip;
            Button.BuyComplete -= FinishBuy;
        }

        public void Equip(UpgradeManager.Upgrade u, UpgradeSlot s, GameObject button)
        {
            if (s != this) { return; }

            ActiveUpgrade = u;
            ActiveUpgradeButton = button;
            _slotText.text = u.Name;
            EventSystem.current.SetSelectedGameObject(gameObject, new BaseEventData(EventSystem.current));
        }

        public void Unequip(UpgradeManager.Upgrade u, UpgradeSlot s)
        {
            if (s != this) { return; }
            ResetSlot();
        }

        public void ResetSlot()
        {
            _slotText.text = "Upgrade";
            ActiveUpgrade = null;
            ActiveUpgradeButton = null;
        }

        public void EquipSlot(UpgradeManager.Upgrade u, GameObject button)
        {
            if (ActiveUpgrade != null && u == ActiveUpgrade) { return; }

            ActiveUpgradeButton = button;
            ActiveUpgrade = u;
            _slotText.text = u.Name;
            EventSystem.current.SetSelectedGameObject(gameObject, new BaseEventData(EventSystem.current));
        }

        public IEnumerator AttemptEquipBuy()
        {
            if (Bank.Instance.AvailableCurrency < _unlockCost)
            {
                SoundManager.Instance.PlayButtonPress(failed: true);
                yield break;
            }

            _buyPanel.SetActive(true);
            _isBuying = true;
            _buyPanel.transform.GetComponentInChildren<TMP_Text>().text = "Buy for $" + _unlockCost + "?";
            EventSystem.current.SetSelectedGameObject(_buyPanel.transform.Find("DoBuy").gameObject, new BaseEventData(EventSystem.current));
            yield return new WaitWhile(() => _isBuying);
            _buyPanel.SetActive(false);
        }

        private void FinishBuy(bool doBuy)
        {
            if (!_isBuying) { return; }

            if (doBuy)
            {
                Bank.Instance.CashTransfer(-_unlockCost);
                UnlockSlot();
            }
            _isBuying = false;
        }

        private void UnlockSlot()
        {
            Unlocked = true;
            GetComponent<Image>().CrossFadeAlpha(1, 0f, true); // doesn't work?
            GetComponent<Image>().DOFade(1f, 0); // using DOTween instead
            transform.Find("Text (TMP)").GetComponent<TMP_Text>().DOFade(1f, 0);
            transform.Find("CostText").gameObject.SetActive(false);
        }
    }
}
