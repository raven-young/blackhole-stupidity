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
        private static UpgradeSlotManager _instance;

        // Property to access the Singleton instance
        public static UpgradeSlotManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (!ES3.KeyExists("UpgradeSlotManager"))
                    {

                        _instance = Resources.Load<UpgradeSlotManager>("_ScriptableObjects/UpgradeSlotManager");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (_instance == null)
                        {
                            _instance = CreateInstance<UpgradeSlotManager>();
                        }
                        ES3.Save("UpgradeSlotManager", _instance);
                        Debug.Log("Saved non-existent UpgradeSlotManager key: " + _instance);
                    }
                    else
                    {
                        _instance = ES3.Load<UpgradeSlotManager>("UpgradeSlotManager");
                        Debug.Log("Loaded UpgradeSlotManager asset: " + _instance);
                    }
                }

                return _instance;
            }

            set => _instance = value;
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

        public Dictionary<int, UpgradeSlotState> UpgradeSlotStates;

        private void OnEnable()
        {
            if (UpgradeSlotStates == null)
            {
                UpgradeSlotStates = new();
            }
        }

        public void SaveSlotState(UpgradeSlot slot)
        {
            if (!UpgradeSlotStates.ContainsKey(slot.SlotNumber))
            {
                UpgradeSlotStates[slot.SlotNumber] = new UpgradeSlotState();
            }

            UpgradeSlotState state = UpgradeSlotStates[slot.SlotNumber];

            state.Unlocked = slot.Unlocked;
            state.ActiveUpgradeButtonName = slot.ActiveUpgradeButton != null ? slot.ActiveUpgradeButton.GetComponent<UpgradeButton>().Upgrade.Name : null;

            if (UpgradeSlotStates[slot.SlotNumber].ActiveUpgradeButtonName != null)
                Debug.Log("saved slot state:" + state.Unlocked + " " + state.ActiveUpgradeButtonName + " " + UpgradeSlotStates[slot.SlotNumber].ActiveUpgradeButtonName);
        }

        public UpgradeSlotState LoadSlotState(UpgradeSlot slot)
        {
            if (!UpgradeSlotStates.ContainsKey(slot.SlotNumber))
            {
                Debug.Log("Adding slot " + slot.SlotNumber + " to upgrade slot dict");
                UpgradeSlotStates[slot.SlotNumber] = slot.ActiveUpgradeButton == null  ? new UpgradeSlotState() : new UpgradeSlotState(slot.Unlocked, slot.ActiveUpgradeButton.GetComponent<UpgradeButton>().Upgrade.Name);
                return UpgradeSlotStates[slot.SlotNumber];
            }
            return UpgradeSlotStates[slot.SlotNumber];
        }

    }
}
