using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole {
    public class UpgradeSlotManager : MonoBehaviour
    {
        public static UpgradeSlotManager Instance;
        [SerializeField] private List<UpgradeSlot> _upgradeslots = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void MakeSlotsInteractable(bool interactable)
        {
            foreach (UpgradeSlot s in _upgradeslots)
            {
                s.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = interactable;
            }
        }
    }
}
