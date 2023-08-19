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

        #region Upgrades
        public Upgrade MagnetUpgrade = new("Super Magnet", "Magnet radius +100%", 1000, ActivateMagnetUpgrade);
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

            if (u.UnlockCost < Bank.Instance.AvailableCurrency)
            {
                u.Unlocked = true;
                Bank.Instance.CashTransfer(-u.UnlockCost);
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
