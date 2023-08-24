using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace BlackHole
{
    public class UpgradeSlot : MonoBehaviour
    {
        public static UpgradeSlotManager _upgradeSlotManagerSO;
        public UpgradeManager.Upgrade ActiveUpgrade;
        public GameObject ActiveUpgradeButton;

        public static Dictionary<int, UpgradeSlot> UpgradeSlots;

        [SerializeField] private Bank _bankSO;
        [SerializeField] private GameObject _buyPanel;
        private TMP_Text _slotText;
        [SerializeField] private int _unlockCost;
        public bool Unlocked;
        private bool _isBuying = false;
        public int SlotNumber;

        private void Awake()
        {
            _slotText = transform.Find("Text (TMP)").GetComponent<TMP_Text>();

            if (UpgradeSlots == null)
            {
                UpgradeSlots = new();
            }

            if (_upgradeSlotManagerSO == null)
            {
                _upgradeSlotManagerSO = Resources.Load<UpgradeSlotManager>("_ScriptableObjects/UpgradeSlotManager");
            }
        }

        private void LoadSlotStateWrapper() // i wanna cri
        {
            UpgradeSlotManager.UpgradeSlotState dummy = _upgradeSlotManagerSO.LoadSlotState(this);
            Debug.Log("unlocked? " + Unlocked);
            Unlocked = dummy.Unlocked;
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadSlotStateWrapper();

            if (Unlocked)
            {
                UnlockSlot(); // change the appearance of slot to unlocked slot
            }
            Debug.Log("Loaded " + "UpgradeSlot" + SlotNumber + " " + ActiveUpgrade + " " + ActiveUpgradeButton);

            if (ActiveUpgrade == null || ActiveUpgrade.Name == "")
            {
                ResetSlot();
            }
            else
            {
                _slotText.text = ActiveUpgrade.Name;
                ActiveUpgradeButton.GetComponent<UpgradeButton>().Equip(ActiveUpgrade, this);
            }

            transform.Find("CostText").GetComponent<TMP_Text>().text = "$" + _unlockCost;
        }

        private void OnEnable()
        {
            UpgradeListDisplay.OnUpgradeEquipped += Equip;
            UpgradeListDisplay.OnUpgradeUnequipped += Unequip;
            Button.BuyComplete += FinishBuy;
            UpgradeSlots[SlotNumber] = this;
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

        public static void ResetAndLockAllSlots()
        {
            foreach (UpgradeSlot slot in UpgradeSlots.Values)
            {
                slot._slotText.text = "Upgrade";
                slot.ActiveUpgrade = null;
                slot.ActiveUpgradeButton = null;
                LockSlot(slot);
            }
        }

        public static void SaveAllSlotStates()
        {
            foreach (UpgradeSlot slot in UpgradeSlots.Values)
            {
                _upgradeSlotManagerSO.SaveSlotState(slot);
            }
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
            if (_bankSO.AvailableCurrency < _unlockCost)
            {
                SoundManager.Instance.PlayButtonPress(failed: true);
                yield break;
            }

            _buyPanel.SetActive(true);
            _isBuying = true;
            _buyPanel.transform.GetComponentInChildren<TMP_Text>().text = "Buy for $" + _unlockCost + "?";
            EventSystem.current.SetSelectedGameObject(_buyPanel.transform.Find("Yes").gameObject, new BaseEventData(EventSystem.current));
            yield return new WaitWhile(() => _isBuying);
            _buyPanel.SetActive(false);
        }

        private void FinishBuy(bool doBuy)
        {
            if (!_isBuying) { return; }

            if (doBuy)
            {
                _bankSO.CashTransfer(-_unlockCost);
                UnlockSlot();
            }
            _isBuying = false;
        }

        private void UnlockSlot()
        {
            Unlocked = true;
            //GetComponent<Image>().CrossFadeAlpha(1, 0f, true); // doesn't work?
            GetComponent<Image>().DOFade(1f, 0); // using DOTween instead
            transform.Find("Text (TMP)").GetComponent<TMP_Text>().DOFade(1f, 0);
            transform.Find("CostText").gameObject.SetActive(false);
            _upgradeSlotManagerSO.SaveSlotState(this);
        }

        private static void LockSlot(UpgradeSlot slot)
        {
            slot.Unlocked = false;
            slot.GetComponent<Image>().DOFade(0.5f, 0);
            slot.transform.Find("Text (TMP)").GetComponent<TMP_Text>().DOFade(0.5f, 0);
            slot.transform.Find("CostText").gameObject.SetActive(true);
            _upgradeSlotManagerSO.SaveSlotState(slot);
        }
    }
}