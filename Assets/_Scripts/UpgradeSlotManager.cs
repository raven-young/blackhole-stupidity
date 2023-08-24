using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BlackHole
{
    [CreateAssetMenu(fileName = "UpgradeSlotManager", menuName = "ScriptableObject/UpgradeSlotManager")]
    public class UpgradeSlotManager : ScriptableObject
    {
        // The Singleton instance
        private static UpgradeSlotManager instance;

        // Property to access the Singleton instance
        public static UpgradeSlotManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!ES3.KeyExists("UpgradeSlotManager"))
                    {

                        instance = Resources.Load<UpgradeSlotManager>("_ScriptableObjects/UpgradeSlotManager");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (instance == null)
                        {
                            instance = CreateInstance<UpgradeSlotManager>();
                        }
                        ES3.Save("UpgradeSlotManager", instance);
                        Debug.Log("Saved non-existent UpgradeSlotManager key: " + instance);
                    }
                    else
                    {
                        instance = ES3.Load<UpgradeSlotManager>("UpgradeSlotManager");
                        Debug.Log("Loaded UpgradeSlotManager asset: " + instance);
                    }
                }

                return instance;
            }
        }

        [Serializable]
        public class UpgradeSlotState
        {
            public UpgradeSlotState(bool unlocked)
            {
                Unlocked = unlocked;
            }
            public bool Unlocked;

        }

        private Dictionary<int, UpgradeSlotState> _upgradeSlotStates;

        private void OnEnable()
        {
            if (_upgradeSlotStates == null)
            {
                _upgradeSlotStates = new();
            }
        }

        public void SaveSlotState(UpgradeSlot slot)
        {
            UpgradeSlotState state = _upgradeSlotStates[slot.SlotNumber];
            state.Unlocked = slot.Unlocked;
        }

        public UpgradeSlotState LoadSlotState(UpgradeSlot slot)
        {
            if (!_upgradeSlotStates.ContainsKey(slot.SlotNumber))
            {
                Debug.Log("Adding slot " + slot.SlotNumber + " to upgrade slot dict");
                _upgradeSlotStates[slot.SlotNumber] = new UpgradeSlotState(slot.Unlocked);
                return _upgradeSlotStates[slot.SlotNumber];
            }
            return _upgradeSlotStates[slot.SlotNumber];
        }

    }
}