using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace BlackHole
{
    [CreateAssetMenu(fileName = "UpgradeManager", menuName = "ScriptableObject/UpgradeManager")]
    public class UpgradeManager : ScriptableObject
    {
        // The Singleton instance
        private static UpgradeManager _instance;

        // Property to access the Singleton instance
        public static UpgradeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("null upgrade manaer, get new");
                    if (!ES3.KeyExists("UpgradeManager"))
                    {

                        _instance = Resources.Load<UpgradeManager>("_ScriptableObjects/UpgradeManager");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (_instance == null)
                        {
                            _instance = CreateInstance<UpgradeManager>();
                        }
                        ES3.Save("UpgradeManager", _instance);
                        Debug.Log("Saved non-existent UpgradeManager key: " + _instance);
                    }
                    else
                    {
                        _instance = ES3.Load<UpgradeManager>("UpgradeManager");
                        Debug.Log("Loaded UpgradeManager asset: " + _instance);
                    }
                }

                return _instance;
            }

            set => _instance = value;
        }

        [SerializeField] private string dummyreminder = "DON'T RENAME UPGRADES UNTIL NULL DELEGATE ISSUE IS PROPERLY FIXED";

        [Serializable]
        public class Upgrade
        {
            public Upgrade(string name, string description, int cost, ActivateUpgrade activate)
            {
                Name = name;
                Description = description;
                Activate = activate;
                UnlockCost = cost;
            }

            public string Name; // don't make readonly (null delegate issue)
            public string Description;
            public int UnlockCost;
            public bool Unlocked = false;
            public bool Equipped = false;
            public int EquippedSlotNumber;

            // Warning: EasySave cannot save this!
            public delegate void ActivateUpgrade(bool unequip);
            public ActivateUpgrade Activate;
        }
        
        public List<Upgrade> AllUpgrades { get; private set; } = new();
        public static event Action OnAllUpgradesUnlocked;
        public float UnlockedUpgradesFraction
        { 
            get => AllUpgrades.Count > 0 ? (float)(_unlockedUpgradesCount) / AllUpgrades.Count : 0f; 
            set => Mathf.Clamp(value, 0f, 1f);
        }

        private int _unlockedUpgradesCount = 0;
        private readonly List<Upgrade> EquippedUpgrades = new();

        private void Awake()
        {
            Debug.Log("awakening upgrade manager");
        }

        private void OnEnable()
        {
            Debug.Log("enable upgrade manager");

            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.Log("Redundant UpgradeManager instance");
                Destroy(this);
            }
        }

        // workaround, refactor save system later
        private void FillNullDelegate(Upgrade u)
        {
            switch (u.Name)
            {
                case "Super Magnet": u.Activate = ActivateMagnetUpgrade; break;
                case "Quickshot": u.Activate = ActivateFirerateUpgrade; break;
                case "Triple Shot": u.Activate = ActivateTripleshotUpgrade; break;
                case "Shield": u.Activate = ActivateShieldUpgrade; break;
                case "Combo Saver": u.Activate = ActivateComboSaverUpgrade; break;
                case "Scavenger": u.Activate = ActivateItemSpawnBonusUpgrade; break;
                case "Slow Beam": u.Activate = ActivateAsteroidSlowUpgrade; break;
            }
        }

        #region Upgrades
        public Upgrade MagnetUpgrade = new("Super Magnet", "Magnet radius +100%", 1000, ActivateMagnetUpgrade);
        private static void ActivateMagnetUpgrade(bool activate = true)
        {
            Debug.Log("pre magnet scale " + SettingsManager.MagnetScale);
            if (SettingsManager.MagnetScale < 10 || SettingsManager.MagnetScale > 20) Debug.LogWarning("fix this finally ffs");
            if (activate)
            {
                SettingsManager.MagnetScale *= 2f;
            }
            else
            {
                SettingsManager.MagnetScale /= 2f;
            }

            Debug.Log("post magnet scale " + SettingsManager.MagnetScale);
        }

        public Upgrade FirerateUpgrade = new("Quickshot", "Firerate +25%", 1000, ActivateFirerateUpgrade);
        private static void ActivateFirerateUpgrade(bool activate = true)
        {
            if (activate)
            {
                SettingsManager.FirePeriod *= 0.75f;
            }
            else
            {
                SettingsManager.FirePeriod /= 0.75f;
            }
        }

        public Upgrade TripleshotUpgrade = new("Triple Shot", "Cannons +1", 3000, ActivateTripleshotUpgrade);
        private static void ActivateTripleshotUpgrade(bool activate = true)
        {
            if (activate)
            {
                SettingsManager.TripleShotEnabled = true;
            }
            else
            {
                SettingsManager.TripleShotEnabled = false;
            }
        }

        public Upgrade ShieldUpgrade = new("Shield", "Absorbs damage, regenerates after 10s", 2000, ActivateShieldUpgrade);
        private static void ActivateShieldUpgrade(bool activate = true)
        {
            if (activate)
            {
                SettingsManager.ShieldEnabled = true;
            }
            else
            {
                SettingsManager.ShieldEnabled = false;
            }
        }

        public Upgrade ComboSaverUpgrade = new("Combo Saver", "Prevents combo loss once", 2000, ActivateComboSaverUpgrade);
        private static void ActivateComboSaverUpgrade(bool activate = true)
        {
            if (activate)
            {
                SettingsManager.ComboSaverEnabled = true;
            }
            else
            {
                SettingsManager.ComboSaverEnabled = false;
            }
        }

        public Upgrade ItemSpawnBonusUpgrade = new("Scavenger", "Dropped items +3", 1500, ActivateItemSpawnBonusUpgrade);
        private static void ActivateItemSpawnBonusUpgrade(bool activate = true)
        {
            if (activate)
            {
                SettingsManager.ItemSpawnBonus += 3;
            }
            else
            {
                SettingsManager.ItemSpawnBonus -= 3;
            }
        }

        public Upgrade AsteroidSlowUpgrade = new("Slow Beam", "Math asteroid speed -20%", 1500, ActivateAsteroidSlowUpgrade);
        private static void ActivateAsteroidSlowUpgrade(bool activate = true)
        {
            if (activate)
            {
                SettingsManager.AsteroidSpeedModifier *= 0.8f;
            }
            else
            {
                SettingsManager.AsteroidSpeedModifier /= 0.8f;
            }
        }

        #endregion

        public void UnlockUpgrade (Upgrade u)
        {
            if (u.Unlocked) { return; }

            if (u.UnlockCost < Bank.AvailableCurrency)
            {
                u.Unlocked = true;
                Bank.CashTransfer(-u.UnlockCost);
                _unlockedUpgradesCount++;
                if (_unlockedUpgradesCount == AllUpgrades.Count)
                {
                    OnAllUpgradesUnlocked.Invoke();
                }
            }
        }

        public void EquipUpgrade(Upgrade u)
        {
            if (!u.Unlocked)
            {
                return;
            }

            if (u.Equipped)
            {
                Debug.LogWarning("Trying to equip upgrade that is already equipped!");
                return;
            }

            EquippedUpgrades.Add(u);
            u.Equipped = true;
            SafeActivate(u, true);
        }

        private void SafeActivate(Upgrade u, bool activate)
        {
            if (u.Activate.Method == null)
            {
                FillNullDelegate(u);
                if (u.Activate.Method == null)
                {
                    Debug.LogError("Attemtping to call null delegate!! This crashes everything, editor included!!!");
                    return;
                }
            }
            u.Activate(activate);
        }

        public void UnequipUpgrade(Upgrade u)
        {
            if (!u.Unlocked)
            {
                return;
            }

            if (!u.Equipped)
            {
                Debug.LogWarning("Trying to unequip upgrade that is not equipped!");
                return;
            }

            if (EquippedUpgrades.Contains(u))
            {
                EquippedUpgrades.Remove(u);
            }
            else
            {
                Debug.LogWarning("Trying to unequip upgrade not in equipped upgrades list");
            }
            u.Equipped = false;
            SafeActivate(u, false);
        }

        public void InitializeUpgrades()
        {
            _unlockedUpgradesCount = 0;
            AllUpgrades = new(this.GetNestedFieldValuesOfType<Upgrade>());
            foreach (Upgrade u in AllUpgrades) { if (u.Unlocked) _unlockedUpgradesCount++; }
        }

        public List<Upgrade> GetAllUpgrades()
        {
            return new(this.GetNestedFieldValuesOfType<Upgrade>());
        }

        public void ResetAllUpgrades()
        {
            foreach (Upgrade u in AllUpgrades)
            {
                u.Unlocked = false;
                u.Equipped = false;
            }
            UnlockedUpgradesFraction = 0f;
            _unlockedUpgradesCount = 0;
            //UpgradeSlot.ResetAndLockAllSlots();
            UpgradeSlotManager.Instance.ResetAllSlots();
            UpgradeListDisplay.Instance.RefreshUpgradeList();
        }
    }
}
