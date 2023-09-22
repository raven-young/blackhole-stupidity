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
        public UpgradeManager.Upgrade ActiveUpgrade;
        public GameObject ActiveUpgradeButton; // redundant, should write method to get button from upgrade
        [SerializeField] private GameObject _activeSlotIndicator;
        [SerializeField] private GameObject _buyPanel;
        public static Dictionary<int, UpgradeSlot> UpgradeSlots;

        private TMP_Text _slotText;
        [SerializeField] private int _unlockCost;
        public bool Unlocked;
        public int SlotNumber;
        private bool _isBuying = false;
        private bool _isActiveSlot;
        public bool IsActiveSlot
        { 
            get => _isActiveSlot; 
            set 
            {
                _isActiveSlot = value;
                Color newcolor = ActiveUpgrade == null ? Color.red : Color.green;
                newcolor.a = _isActiveSlot ? 1f : 0.5f;
                _activeSlotIndicator.GetComponent<Image>().color = newcolor;

                if (!_isActiveSlot) return;
                
                foreach (UpgradeSlot slot in UpgradeSlots.Values)
                {
                    if (slot != this)
                    {
                        slot.IsActiveSlot = false;
                        slot._activeSlotIndicator.GetComponent<Image>().DOFade(0.5f, 0f);
                    }
                }
            } 
        }
        
        private void Awake()
        {
            _slotText = transform.Find("Text (TMP)").GetComponent<TMP_Text>();

            if (UpgradeSlots == null)
            {
                UpgradeSlots = new();
            }

            if (UpgradeSlotManager.Instance == null)
            {
                Debug.LogWarning("UpgradeSlot: UpgradeSlotManager is null");
                UpgradeSlotManager.Instance = Resources.Load<UpgradeSlotManager>("_ScriptableObjects/UpgradeSlotManager");
            }

            UpgradeSlots[SlotNumber] = this;
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

        private void LoadSlotStateWrapper() // i wanna cri
        {
            UpgradeSlotManager.UpgradeSlotState dummy = UpgradeSlotManager.Instance.LoadSlotState(this);
            Debug.Log("slot loaded, unlocked? " + Unlocked + " button: " + dummy.ActiveUpgradeButtonName);
            Unlocked = dummy.Unlocked;
            if (dummy.ActiveUpgradeButtonName != null && dummy.ActiveUpgradeButtonName != "")
            {
                ActiveUpgradeButton = UpgradeListDisplay.GetUpgradeButtonFromName(dummy.ActiveUpgradeButtonName);
                ActiveUpgrade = ActiveUpgradeButton.GetComponent<UpgradeButton>().Upgrade;
            } 
            else
            {
                ActiveUpgradeButton = null;
                ActiveUpgrade = null;
            }
        }

        public static void Init()
        {
            foreach (UpgradeSlot slot in UpgradeSlots.Values)
            {
                slot.LoadSlotStateWrapper();

                if (slot.Unlocked)
                {
                    slot.UnlockSlot(); // change the appearance of slot to unlocked slot
                }
                Debug.Log("Loaded " + "UpgradeSlot" + slot.SlotNumber + " " + slot.ActiveUpgrade + " " + slot.ActiveUpgradeButton);

                if (slot.ActiveUpgrade == null || slot.ActiveUpgrade.Name == "")
                {
                    slot.ResetSlot();
                }
                else
                {
                    slot._slotText.text = slot.ActiveUpgrade.Name;
                    slot.ActiveUpgradeButton.GetComponent<UpgradeButton>().EquippedSlot = slot;
                    slot.ActiveUpgradeButton.GetComponent<UpgradeButton>().Equip(slot.ActiveUpgrade, slot);
                }

                slot.transform.Find("CostText").GetComponent<TMP_Text>().text = "$" + slot._unlockCost;
            }
        }

        public void Equip(UpgradeManager.Upgrade u, UpgradeSlot s, GameObject button)
        {
            if (s != this) { return; }

            ActiveUpgrade = u;
            ActiveUpgradeButton = button;
            _slotText.text = u.Name;
            _activeSlotIndicator.GetComponent<Image>().color = Color.green;
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
            Color color = Color.red;
            color.a = 0.5f;
            _activeSlotIndicator.GetComponent<Image>().color = color;
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
                UpgradeSlotManager.Instance.SaveSlotState(slot);
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

        public void AttemptEquipBuyWrapper()
        {
            StartCoroutine(AttemptEquipBuy());
        }

        public IEnumerator AttemptEquipBuy()
        {
            if (Bank.AvailableCurrency < _unlockCost)
            {
                SoundManager.Instance.PlayButtonPress(failed: true);
                yield break;
            }

            _buyPanel.SetActive(true);
            _isBuying = true;
            _buyPanel.GetComponent<BuyPanel>().HeaderText.text = "Buy for $" + _unlockCost + "?";
            yield return new WaitWhile(() => _isBuying);
            _buyPanel.SetActive(false);
        }

        private void FinishBuy(bool doBuy)
        {
            if (!_isBuying) { return; }

            if (doBuy)
            {
                Bank.CashTransfer(-_unlockCost);
                UnlockSlot();
                UpgradeSlotManager.Instance.SwitchSelectedUpgradeSlot(this);
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
            UpgradeSlotManager.Instance.SaveSlotState(this);
        }

        public static void LockSlot(UpgradeSlot slot)
        {
            slot.Unlocked = false;
            slot.GetComponent<Image>().DOFade(0.5f, 0);
            slot.transform.Find("Text (TMP)").GetComponent<TMP_Text>().DOFade(0.5f, 0);
            slot.transform.Find("CostText").gameObject.SetActive(true);
            UpgradeSlotManager.Instance.SaveSlotState(slot);
        }
    }
}
