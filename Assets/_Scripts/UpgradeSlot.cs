using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace BlackHole
{
    public class UpgradeSlot : MonoBehaviour
    {
        public UpgradeManager.Upgrade ActiveUpgrade;
        private TMP_Text _slotText;

        public GameObject ActiveUpgradeButton;

        private void Awake()
        {
            _slotText = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (ActiveUpgrade == null || ActiveUpgrade.Name == "")
            {
                ResetSlot();
            }
        }

        private void OnEnable()
        {
            UpgradeListDisplay.OnUpgradeEquipped += Equip;
            UpgradeListDisplay.OnUpgradeUnequipped += Unequip;
        }

        private void OnDisable()
        {
            UpgradeListDisplay.OnUpgradeEquipped -= Equip;
            UpgradeListDisplay.OnUpgradeUnequipped -= Unequip;
        }

        public void Equip(UpgradeManager.Upgrade u, UpgradeSlot s, GameObject button)
        {
            if (s != this) { return; }

            ActiveUpgrade = u;
            ActiveUpgradeButton = button;
            _slotText.text = u.Name;
            EventSystem.current.SetSelectedGameObject(gameObject, new BaseEventData(EventSystem.current));
        }

        public void Unequip(UpgradeManager.Upgrade u, UpgradeSlot s)
        {
            if (s != this) { return; }
            ResetSlot();
        }

        public void ResetSlot()
        {
            _slotText.text = "Upgrade";
            ActiveUpgrade = null;
            ActiveUpgradeButton = null;
        }

        public void EquipSlot(UpgradeManager.Upgrade u, GameObject button)
        {
            if (ActiveUpgrade != null && u == ActiveUpgrade) { return; }

            ActiveUpgradeButton = button;
            ActiveUpgrade = u;
            _slotText.text = u.Name;
            EventSystem.current.SetSelectedGameObject(gameObject, new BaseEventData(EventSystem.current));
        }
    }
}
