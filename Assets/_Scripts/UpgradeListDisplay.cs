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
            UpgradeButton.UpgradeButtonClick += OnUpgradeButtonClick;
        }

        private void OnDisable()
        {
            Button.BuyComplete -= FinishBuy;
            UpgradeButton.UpgradeButtonClick -= OnUpgradeButtonClick;
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

        public void OnUpgradeButtonClick(GameObject button)
        {
            UpgradeManager.Upgrade u = button.transform.GetComponent<UpgradeButton>().Upgrade;

            if (!u.Unlocked) {
                StartCoroutine(AttemptEquipBuy(u));
                if (!u.Unlocked) { return; } 
            }

            if (UpgradeSlotManager.Instance.SelectedUpgradeSlot == null) {
                Debug.LogWarning("Selected upgrade slot is null");
                return; 
            }

            if (!UpgradeSlotManager.Instance.SelectedUpgradeSlot.Unlocked)
            {
                SoundManager.Instance.PlayButtonPress(failed: true);
                return;
            }

            SoundManager.Instance.PlayButtonPress(failed: false);

            // if equipped to different slot, unequip there
            if (u.Equipped && UpgradeSlotManager.Instance.SelectedUpgradeSlot.SlotNumber != u.EquippedSlotNumber) {
                OnUpgradeUnequipped?.Invoke(u, UpgradeSlot.UpgradeSlots[u.EquippedSlotNumber]);
            }

            // if already equipped to this slot, unequip the upgrade
            if (u.Equipped && UpgradeSlotManager.Instance.SelectedUpgradeSlot.SlotNumber == u.EquippedSlotNumber)
            {
                OnUpgradeUnequipped?.Invoke(u, UpgradeSlotManager.Instance.SelectedUpgradeSlot);
                EventSystem.current.SetSelectedGameObject(UpgradeSlotManager.Instance.SelectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
                Debug.Log("unequppped, now selected slot: " + UpgradeSlotManager.Instance.SelectedUpgradeSlot);
            }
            else if (!u.Equipped)
            {
                u.EquippedSlotNumber = UpgradeSlotManager.Instance.SelectedUpgradeSlot.SlotNumber;
                OnUpgradeEquipped?.Invoke(u, UpgradeSlotManager.Instance.SelectedUpgradeSlot, button);
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
            EventSystem.current.SetSelectedGameObject(UpgradeSlotManager.Instance.SelectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
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

            SoundManager.Instance.PlayButtonPress(failed: false);
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
