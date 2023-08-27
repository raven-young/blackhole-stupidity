using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace BlackHole
{
    [CreateAssetMenu(fileName = "UpgradeSlotManager", menuName = "ScriptableObject/UpgradeSlotManager")]
    public class UpgradeSlotManager : SerializedScriptableObject
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
            public UpgradeSlotState()
            {

            }
            public UpgradeSlotState(bool unlocked, string activeUpgradeButtonName)
            {
                Unlocked = unlocked;
                ActiveUpgradeButtonName = activeUpgradeButtonName;
            }
            public bool Unlocked;
            public string ActiveUpgradeButtonName;
        }

        [SerializeField] private Dictionary<int, UpgradeSlotState> _upgradeSlotStates;

        private void OnEnable()
        {
            if (_upgradeSlotStates == null)
            {
                _upgradeSlotStates = new();
            }
        }

        public void SaveSlotState(UpgradeSlot slot)
        {
            if (!_upgradeSlotStates.ContainsKey(slot.SlotNumber))
            {
                _upgradeSlotStates[slot.SlotNumber] = new UpgradeSlotState();
            }

            UpgradeSlotState state = _upgradeSlotStates[slot.SlotNumber];

            state.Unlocked = slot.Unlocked;
            state.ActiveUpgradeButtonName = slot.ActiveUpgradeButton != null ? slot.ActiveUpgradeButton.GetComponent<UpgradeButton>().Upgrade.Name : null;

            if (_upgradeSlotStates[slot.SlotNumber].ActiveUpgradeButtonName != null)
                Debug.Log("saved slot state:" + state.Unlocked + " " + state.ActiveUpgradeButtonName + " " + _upgradeSlotStates[slot.SlotNumber].ActiveUpgradeButtonName);
        }

        public UpgradeSlotState LoadSlotState(UpgradeSlot slot)
        {
            if (!_upgradeSlotStates.ContainsKey(slot.SlotNumber))
            {
                Debug.Log("Adding slot " + slot.SlotNumber + " to upgrade slot dict");
                _upgradeSlotStates[slot.SlotNumber] = slot.ActiveUpgradeButton == null  ? new UpgradeSlotState() : new UpgradeSlotState(slot.Unlocked, slot.ActiveUpgradeButton.GetComponent<UpgradeButton>().Upgrade.Name);
                return _upgradeSlotStates[slot.SlotNumber];
            }
            return _upgradeSlotStates[slot.SlotNumber];
        }

    }
}
