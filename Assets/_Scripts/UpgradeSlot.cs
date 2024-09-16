using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MoreMountains.Tools;

namespace BlackHole
{
    public class UpgradeSlot : MonoBehaviour
    {
        [SerializeField] private GameObject _activeSlotIndicator;
        [SerializeField] private GameObject _buyPanel;
        public static Dictionary<int, UpgradeSlot> UpgradeSlots;

        private TMP_Text _slotText;
        [SerializeField] private int _unlockCost;
        public bool Unlocked;
        public int SlotNumber;
        private bool _isBuying = false;
        [SerializeField] private bool _hasUpgrade = false;
        public bool HasUpgrade { get => _hasUpgrade; set => _hasUpgrade = value; }
        [SerializeField] private bool _isActiveSlot;
        public bool IsActiveSlot
        { 
            get => _isActiveSlot; 
            set 
            {
                _isActiveSlot = value;
                HandleActiveSlotIndicator();
            } 
        }
        
        private void Awake()
        {
            _slotText = transform.Find("Text (TMP)").GetComponent<TMP_Text>();

            UpgradeSlots ??= new();

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

        public static void Init()
        {
            foreach (UpgradeSlot slot in UpgradeSlots.Values)
            {
                slot.Unlocked = UpgradeSlotManager.Instance.LoadSlotState(slot).Unlocked;

                if (slot.Unlocked)
                {
                    slot.UnlockSlot(); // change the appearance of slot to unlocked slot
                }

                //if (slot.ActiveUpgrade == null || slot.ActiveUpgrade.Name == "")
                //{
                //    slot.ResetSlot();
                //}

                slot.transform.Find("CostText").GetComponent<TMP_Text>().text = "$" + slot._unlockCost;
            }

            UpgradeSlotManager.Instance.SwitchSelectedUpgradeSlot(UpgradeSlots.First().Value, firstTimeSelection: true);

        }

        public void Equip(UpgradeManager.Upgrade u, UpgradeSlot s, GameObject button)
        {
            if (s != this) { return; }

            HasUpgrade = true;
            _slotText.text = u.Name;
            HandleActiveSlotIndicator();
            EventSystem.current.SetSelectedGameObject(gameObject, new BaseEventData(EventSystem.current));
        }

        public void Unequip(UpgradeManager.Upgrade u, UpgradeSlot s)
        {
            if (s != this) { return; }
            ResetSlot();
        }

        private void HandleActiveSlotIndicator()
        {
            Color newcolor = _hasUpgrade ? Color.green : Color.red;
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

        public void ResetSlot()
        {
            _slotText.text = "Upgrade";
            HasUpgrade = false;
            HandleActiveSlotIndicator();
        }

        public static void ResetAndLockAllSlots()
        {
            foreach (UpgradeSlot slot in UpgradeSlots.Values)
            {
                slot._slotText.text = "Upgrade";
                slot.HasUpgrade = false;
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

            SoundManager.Instance.PlayButtonPress(failed: false);
            _buyPanel.SetActive(true);
            _isBuying = true;
            _buyPanel.GetComponent<BuyPanel>().HeaderText.text = "Buy for $" + _unlockCost + "?";
            EventSystem.current.SetSelectedGameObject(_buyPanel.transform.Find("Yes").gameObject, new BaseEventData(EventSystem.current));
            yield return new WaitWhile(() => _isBuying);
            _buyPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(UpgradeSlotManager.Instance.SelectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
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
            SoundManager.Instance.PlayButtonPress(failed: false);
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
