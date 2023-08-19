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
        private static UpgradeManager instance;

        // Property to access the Singleton instance
        public static UpgradeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!ES3.KeyExists("AchievementsManager"))
                    {

                        instance = Resources.Load<UpgradeManager>("_ScriptableObjects/UpgradeManager");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (instance == null)
                        {
                            instance = CreateInstance<UpgradeManager>();
                        }
                        ES3.Save("AchievementsManager", instance);
                        Debug.Log("Saved non-existent UpgradeManager key: " + instance);
                    }
                    else
                    {
                        instance = ES3.Load<UpgradeManager>("UpgradeManager");
                        Debug.Log("Loaded UpgradeManager asset: " + instance);
                    }
                }

                return instance;
            }
        }

        public int AvailableCurrency = 3000;
        private TMP_Text _currencyText;

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

            public string Name;
            public string Description;
            public int UnlockCost;
            public bool Unlocked = false;
            public bool Equipped = false;
            public delegate void ActivateUpgrade(bool unequip);
            public ActivateUpgrade Activate;
        }

        public List<Upgrade> AllUpgrades { get; private set; } = new();
        private readonly List<Upgrade> EquippedUpgrades = new();

        private void OnEnable()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Redundant UpgradeManager instance");
                Destroy(this);
            }
        }

        public void Init()
        {
            _currencyText = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Currency").GetComponent<TMP_Text>();
            Debug.Log("pay day " + _currencyText);
            AvailableCurrency = 10000;
            _currencyText.text = "$"+AvailableCurrency.ToString();
        }

        #region Upgrades
        public Upgrade MagnetUpgrade = new("Magnet I", "Magnet radius +100%", 1000, ActivateMagnetUpgrade);
        private static void ActivateMagnetUpgrade(bool activate = true)
        {
            Debug.Log("pre magnet scale " + SettingsManager.MagnetScale);
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

        public Upgrade FirerateUpgrade = new("Firerate I", "Firerate +25%", 1000, ActivateFirerateUpgrade);
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

        public Upgrade ShieldUpgrade = new("Shield", "Absorbs asteroid damage\nRegenerates after 10s", 2000, ActivateShieldUpgrade);
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

        #endregion

        public void UnlockUpgrade (Upgrade u)
        {
            if (u.Unlocked) { return; }

            if (u.UnlockCost < AvailableCurrency)
            {
                u.Unlocked = true;
                AvailableCurrency -= u.UnlockCost;
                _currencyText.text = AvailableCurrency.ToString();
            }
        }

        public void EquipUpgrade(Upgrade u)
        {
            if (!u.Unlocked)
            {
                return;
            }
            EquippedUpgrades.Add(u);
            u.Equipped = true;
            u.Activate(true);
        }

        public void UnequipUpgrade(Upgrade u)
        {
            if (!u.Unlocked)
            {
                return;
            }
            EquippedUpgrades.Remove(u);
            u.Equipped = false;
            u.Activate(false);
        }

        // since upgrade button is a prefab instantiate at run-time, call wrapper from this persisent scriptable object
        public void EquipUpgradeFromUpgradeButtonWrapper(GameObject button)
        {
            UpgradeListDisplay.Instance.EquipUpgradeFromUpgradeButton(button);
        }
        public void InitializeUpgrades()
        {
            AllUpgrades = new(this.GetNestedFieldValuesOfType<Upgrade>());
        }

        public List<Upgrade> GetAllUpgrades()
        {
            return new(this.GetNestedFieldValuesOfType<Upgrade>());
        }

        public void LockAllUpgrades()
        {
            foreach (Upgrade u in AllUpgrades)
            {
                u.Unlocked = false;
            }

            UpgradeListDisplay.Instance.RefreshUpgradeList();
        }
    }
}
