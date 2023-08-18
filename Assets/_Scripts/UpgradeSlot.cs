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

        public void ResetSlot()
        {
            _slotText.text = "Upgrade";
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
