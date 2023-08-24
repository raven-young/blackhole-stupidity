using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using DG.Tweening;

namespace BlackHole
{
    public class UpgradeListDisplay : MonoBehaviour
    {
        public static UpgradeListDisplay Instance;
        [SerializeField] private Bank _bankSO;
        [SerializeField] private GameObject _viewPortContent;
        private List<UpgradeManager.Upgrade> _allUpgrades;
        [SerializeField] private GameObject _upgradeButtonPrefab;
        [SerializeField] private TMP_Text _upgradeCounterText;
        [SerializeField] private UpgradeSlot _selectedUpgradeSlot;
        [SerializeField] private GameObject _buyPanel;
        public static event Action<UpgradeManager.Upgrade, UpgradeSlot, GameObject> OnUpgradeEquipped;
        public static event Action<UpgradeManager.Upgrade, UpgradeSlot> OnUpgradeUnequipped;
        public static event Action<UpgradeManager.Upgrade> OnUpgradeBought;

        private bool _isBuying = false;
        private UpgradeManager.Upgrade _buyCandidate;

        private List<UpgradeButton> _upgradeButtons = new();

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
                    OnUpgradeEquipped?.Invoke(u, UpgradeSlot.UpgradeSlots[u.EquippedSlotNumber], button);
                }
                _upgradeButtons.Add(button.GetComponent<UpgradeButton>());
            }

            _upgradeCounterText.text = "Unlocked: " + Math.Round(100f * UpgradeManager.Instance.UnlockedUpgradesFraction) + "%";
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
        public void SwitchSelectedUpgradeSlot(UpgradeSlot newslot)
        {
            Debug.Log("newslot: " + newslot + " " + newslot.Unlocked);
            if (!newslot.Unlocked) {
                StartCoroutine(newslot.AttemptEquipBuy());
                if (!newslot.Unlocked) { return; }
            }

            _selectedUpgradeSlot = newslot;

            // if new slot already has equipped upgrade, jump to corresponding upgrade button
            if (_selectedUpgradeSlot.ActiveUpgrade != null && _selectedUpgradeSlot.ActiveUpgradeButton != null)
            {
                Debug.Log("switch to upgrade: " + _selectedUpgradeSlot.ActiveUpgradeButton);
                EventSystem.current.SetSelectedGameObject(_selectedUpgradeSlot.ActiveUpgradeButton, new BaseEventData(EventSystem.current));
            }
            // else jump to some other upgrade button
            else
            {
                //GameObject firstbutton = transform.Find("Scroll View").transform.Find("Viewport").transform.GetComponentInChildren<UpgradeButton>().gameObject;
                GameObject firstbutton = transform.GetComponentInChildren<UpgradeButton>().gameObject;
                Debug.Assert(firstbutton != null);
                Debug.Log("switch first: " + firstbutton);
                EventSystem.current.SetSelectedGameObject(firstbutton, new BaseEventData(EventSystem.current));
            }
            Debug.Log("current event: " + EventSystem.current);

        }

        public void EquipUpgradeFromUpgradeButton(GameObject button)
        {
            UpgradeManager.Upgrade u = button.transform.GetComponent<UpgradeButton>().Upgrade;

            Debug.Log(u.Name + " " + u.Unlocked);

            if (!u.Unlocked) {
                StartCoroutine(AttemptEquipBuy(u));
                if (!u.Unlocked) { Debug.Log("returning");return; } 
            }

            if (!_selectedUpgradeSlot.Unlocked) { return; }

            // to do: if equipped to different slot, unequip there and equip here; for now, return
            if (u.Equipped && _selectedUpgradeSlot.ActiveUpgrade != u) { return; }

            // if already equipped to this slot, unequip the upgrade
            if (u.Equipped && _selectedUpgradeSlot.ActiveUpgrade == u)
            {
                UpgradeManager.Instance.UnequipUpgrade(u);
                //_selectedUpgradeSlot.ResetSlot();
                //_selectedUpgradeSlot.ActiveUpgradeButton = null;
                OnUpgradeUnequipped?.Invoke(u, _selectedUpgradeSlot);
                EventSystem.current.SetSelectedGameObject(_selectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
                Debug.Log("unequppped, now selected slot: " + _selectedUpgradeSlot);
            }
            else
            {
                UpgradeManager.Instance.EquipUpgrade(u);
                u.EquippedSlotNumber = _selectedUpgradeSlot.SlotNumber;
                OnUpgradeEquipped?.Invoke(u, _selectedUpgradeSlot, button);
                //_selectedUpgradeSlot.EquipSlot(u, button);
                EventSystem.current.SetSelectedGameObject(_selectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
            }
        }

        public IEnumerator AttemptEquipBuy(UpgradeManager.Upgrade u)
        {
            if (_bankSO.AvailableCurrency < u.UnlockCost)
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
