using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace BlackHole
{
    public class UpgradeListDisplay : MonoBehaviour
    {
        public static UpgradeListDisplay Instance;

        [SerializeField] private GameObject _viewport;
        private List<UpgradeManager.Upgrade> _allUpgrades;
        [SerializeField] private GameObject _upgradeButtonPrefab;

        [SerializeField] private UpgradeSlot _selectedUpgradeSlot;
        [SerializeField] private GameObject _buyPanel;
        public static event Action<UpgradeManager.Upgrade, UpgradeSlot> OnUpgradeEquipped;
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

        // Start is called before the first frame update
        void Start()
        {
            _allUpgrades = UpgradeManager.Instance.GetAllUpgrades();

            Debug.Log("unequipping all");
            foreach (UpgradeManager.Upgrade u in _allUpgrades)
            {
                GameObject button = Instantiate(_upgradeButtonPrefab, _viewport.transform);
                u.Equipped = false;
                UpgradeManager.Instance.UnequipUpgrade(u);
                button.GetComponent<UpgradeButton>().Initialize(u);
                _upgradeButtons.Add(button.GetComponent<UpgradeButton>());
            }
        }

        public void RefreshUpgradeList()
        {
            foreach (UpgradeButton b in _upgradeButtons)
            {
                b.Initialize(b.Upgrade);
            }
        }
            
        // careful when renaming: upgrade slot OnClick method must be set again
        public void Switch_selectedUpgradeSlot(UpgradeSlot newslot)
        {
            _selectedUpgradeSlot = newslot;

            Debug.Log("switch selection: " + _selectedUpgradeSlot.ActiveUpgrade.Name + " " + _selectedUpgradeSlot.ActiveUpgradeButton);

            // if new slot already has equipped upgrade, jump to corresponding upgrade button
            if (_selectedUpgradeSlot.ActiveUpgrade.Name != "" && _selectedUpgradeSlot.ActiveUpgradeButton != null)
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
                if (!u.Unlocked) { return; } 
            }

            
            // if already equipped, unequip the upgrade
            if (u.Equipped)
            {
                UpgradeManager.Instance.UnequipUpgrade(u);
                _selectedUpgradeSlot.ResetSlot();
                _selectedUpgradeSlot.ActiveUpgradeButton = null;
                OnUpgradeUnequipped?.Invoke(u, _selectedUpgradeSlot);
                EventSystem.current.SetSelectedGameObject(_selectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
                Debug.Log("unequppped, now selected slot: " + _selectedUpgradeSlot);
            }
            else
            {
                UpgradeManager.Instance.EquipUpgrade(u);
                OnUpgradeEquipped?.Invoke(u, _selectedUpgradeSlot);
                _selectedUpgradeSlot.EquipSlot(u, button);
                EventSystem.current.SetSelectedGameObject(_selectedUpgradeSlot.gameObject, new BaseEventData(EventSystem.current));
            }
        }

        public IEnumerator AttemptEquipBuy(UpgradeManager.Upgrade u)
        {
            if (UpgradeManager.Instance.AvailableCurrency < u.UnlockCost)
            {
                Debug.Log("not enough cash: " + UpgradeManager.Instance.AvailableCurrency + " " + u.UnlockCost);
                yield return null;
            }

            _buyCandidate = u;
            _buyPanel.SetActive(true);
            _isBuying = true;
            Debug.Log("start buy");

            yield return new WaitWhile(() => _isBuying);

            Debug.Log("FinishBuy buy");
            _buyPanel.SetActive(false);
        }

        public void FinishBuy(bool doBuy)
        {
            _buyPanel.transform.GetComponentInChildren<TMP_Text>().text = "Buy for $" + _buyCandidate.UnlockCost + "?";
            if (doBuy)
            {
                UpgradeManager.Instance.UnlockUpgrade(_buyCandidate);
                OnUpgradeBought?.Invoke(_buyCandidate);
            }

            _isBuying = false;
            Debug.Log("isbuying " + _isBuying);
        }

    }
}
