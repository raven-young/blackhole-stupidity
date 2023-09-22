using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

namespace BlackHole
{
    public class UpgradeListDisplay : MonoBehaviour
    {
        public static UpgradeListDisplay Instance;

        public static event Action<UpgradeManager.Upgrade, UpgradeSlot, GameObject> OnUpgradeEquipped;
        public static event Action<UpgradeManager.Upgrade, UpgradeSlot> OnUpgradeUnequipped;
        public static event Action<UpgradeManager.Upgrade> OnUpgradeBought;

        [SerializeField] private GameObject _viewPortContent;
        [SerializeField] private GameObject _upgradeButtonPrefab;
        [SerializeField] private TMP_Text _upgradeCounterText;
        [SerializeField] private GameObject _buyPanel;

        private List<UpgradeManager.Upgrade> _allUpgrades;
        private bool _isBuying = false;
        private UpgradeManager.Upgrade _buyCandidate;
        private static List<UpgradeButton> _upgradeButtons = new();

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

            _upgradeButtons = new();
        }

        private void OnEnable()
        {
            Button.BuyComplete += FinishBuy;
        }

        private void OnDisable()
        {
            Button.BuyComplete -= FinishBuy;
        }

        // Start is called before the first frame update
        void Start()
        {
            _allUpgrades = UpgradeManager.Instance.GetAllUpgrades();

            SettingsManager.Instance.ResetGameParams();

            foreach (UpgradeManager.Upgrade u in _allUpgrades)
            {
                GameObject button = Instantiate(_upgradeButtonPrefab, _viewPortContent.transform);
                button.GetComponent<UpgradeButton>().Initialize(u);
                if (u.Equipped)
                {
                    u.Equipped = false; // set to true via OnUpgradeEquipped -> UpgradeButton -> Equip
                    OnUpgradeEquipped?.Invoke(u, UpgradeSlot.UpgradeSlots[u.EquippedSlotNumber], button);
                }
                _upgradeButtons.Add(button.GetComponent<UpgradeButton>());
            }

            _upgradeCounterText.text = "Unlocked: " + Math.Round(100f * UpgradeManager.Instance.UnlockedUpgradesFraction) + "%";

            UpgradeSlot.Init();
        }


        public static GameObject GetUpgradeButtonFromName(string name)
        {
            foreach (UpgradeButton b in _upgradeButtons)
            {
                if (b.Upgrade.Name == name) { return b.gameObject; }
            }

            Debug.LogWarning($"Button with name {name} not found");
            return null;
        }

        public void RefreshUpgradeList()
        {
            foreach (UpgradeButton b in _upgradeButtons)
            {
                b.Initialize(b.Upgrade);
            }
            _upgradeCounterText.text = "Unlocked: " + Math.Round(100f * UpgradeManager.Instance.UnlockedUpgradesFraction) + "%";
        }
            
        // careful when renaming: upgrade slot OnClick method must be set again
        //public void SwitchSelectedUpgradeSlot(UpgradeSlot newslot)
        //{
        //    Debug.Log("newslot: " + newslot + " " + newslot.Unlocked);
        //    if (!newslot.Unlocked) {
        //        StartCoroutine(newslot.AttemptEquipBuy());
        //        if (!newslot.Unlocked) { return; }
        //    }

        //    UpgradeSlotManager.Instance.SelectedUpgradeSlot = newslot;
        //    UpgradeSlotManager.Instance.SelectedUpgradeSlot.IsActiveSlot = true; 

        //    // if new slot already has equipped upgrade, jump to corresponding upgrade button
        //    if (UpgradeSlotManager.Instance.SelectedUpgradeSlot.ActiveUpgrade != null && UpgradeSlotManager.Instance.SelectedUpgradeSlot.ActiveUpgradeButton != null)
        //    {
        //        Debug.Log("switch to upgrade: " + UpgradeSlotManager.Instance.SelectedUpgradeSlot.ActiveUpgradeButton);
        //        EventSystem.current.SetSelectedGameObject(UpgradeSlotManager.Instance.SelectedUpgradeSlot.ActiveUpgradeButton, new BaseEventData(EventSystem.current));
        //    }
        //    // else jump to some other upgrade button
        //    else
        //    {
        //        //GameObject firstbutton = transform.Find("Scroll View").transform.Find("Viewport").transform.GetComponentInChildren<UpgradeButton>().gameObject;
        //        GameObject firstbutton = transform.GetComponentInChildren<UpgradeButton>().gameObject;
        //        Debug.Assert(firstbutton != null);
        //        Debug.Log("switch first: " + firstbutton);
        //        EventSystem.current.SetSelectedGameObject(firstbutton, new BaseEventData(EventSystem.current));
        //    }
        //    Debug.Log("current event: " + EventSystem.current);

        //}

        public void UpgradeButtonClick(GameObject button)
        {
            UpgradeManager.Upgrade u = button.transform.GetComponent<UpgradeButton>().Upgrade;

            if (!u.Unlocked) {
                StartCoroutine(AttemptEquipBuy(u));
                if (!u.Unlocked) { Debug.Log("returning");return; } 
            }

            if (UpgradeSlotManager.Instance.SelectedUpgradeSlot == null) {
                Debug.LogWarning("Selected upgrade slot is null");
                return; 
            }

            if (!UpgradeSlotManager.Instance.SelectedUpgradeSlot.Unlocked)
            {
                return;
            }

            // if equipped to different slot, unequip there
            if (u.Equipped && UpgradeSlotManager.Instance.SelectedUpgradeSlot.SlotNumber != u.EquippedSlotNumber) {
                OnUpgradeUnequipped?.Invoke(u, UpgradeSlot.UpgradeSlots[u.EquippedSlotNumber]);
            }

            // if already equipped to this slot, unequip the upgrade
            if (u.Equipped && UpgradeSlotManager.Instance.SelectedUpgradeSlot.SlotNumber == u.EquippedSlotNumber)
            {
                //UpgradeManager.Instance.UnequipUpgrade(u);
                //UpgradeSlotManager.Instance.SelectedUpgradeSlot.ResetSlot();
                //UpgradeSlotManager.Instance.SelectedUpgradeSlot.ActiveUpgradeButton = null;
                OnUpgradeUnequipped?.Invoke(u, UpgradeSlotManager.Instance.SelectedUpgradeSlot);
                EventSystem.current.SetSelectedGameObject(UpgradeSlotManager.Instance.SelectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
                Debug.Log("unequppped, now selected slot: " + UpgradeSlotManager.Instance.SelectedUpgradeSlot);
            }
            else if (!u.Equipped)
            {
                //UpgradeManager.Instance.EquipUpgrade(u);
                u.EquippedSlotNumber = UpgradeSlotManager.Instance.SelectedUpgradeSlot.SlotNumber;
                OnUpgradeEquipped?.Invoke(u, UpgradeSlotManager.Instance.SelectedUpgradeSlot, button);
                //UpgradeSlotManager.Instance.SelectedUpgradeSlot.EquipSlot(u, button);
                EventSystem.current.SetSelectedGameObject(UpgradeSlotManager.Instance.SelectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
            }
            else
            {
                Debug.LogWarning("wat");
            }
        }

        public IEnumerator AttemptEquipBuy(UpgradeManager.Upgrade u)
        {
            if (Bank.AvailableCurrency < u.UnlockCost)
            {
                SoundManager.Instance.PlayButtonPress(failed: true);
                yield break;
            }

            _buyCandidate = u;
            _buyPanel.SetActive(true);
            _isBuying = true;
            _buyPanel.transform.GetComponentInChildren<TMP_Text>().text = "Buy for $" + _buyCandidate.UnlockCost + "?";
            EventSystem.current.SetSelectedGameObject(_buyPanel.transform.Find("Yes").gameObject, new BaseEventData(EventSystem.current));
            yield return new WaitWhile(() => _isBuying);

            _buyPanel.SetActive(false);
        }

        public void FinishBuy(bool doBuy)
        {
            if (!_isBuying) { return; }

            if (doBuy)
            {
                UpgradeManager.Instance.UnlockUpgrade(_buyCandidate);
                OnUpgradeBought?.Invoke(_buyCandidate);
                _upgradeCounterText.text = "Unlocked: " + Math.Round(100f * UpgradeManager.Instance.UnlockedUpgradesFraction) + "%";
            }

            _isBuying = false;
        }

        public void EnterUgradeListPanel()
        {
            gameObject.GetComponent<RectTransform>().DOAnchorPosX(280, 0.2f);
        }
        public void ExitUgradeListPanel()
        {
            gameObject.GetComponent<RectTransform>().DOAnchorPosX(800, 0.2f);
        }
    }
}
