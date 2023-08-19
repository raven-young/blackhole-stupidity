using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace BlackHole {
    public class UpgradeButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public UpgradeManager.Upgrade Upgrade;
        public bool Equipped;
        public UpgradeSlot EquippedSlot;
        private Color _unlockedColor;
        private Image _image;
        private TMP_Text _name;
        private TMP_Text _description;
        private TMP_Text _cost;

        private void Awake()
        {
            ColorUtility.TryParseHtmlString("#0D086F", out _unlockedColor);
            _image = gameObject.GetComponent<Image>();
            _name = gameObject.transform.Find("Name").GetComponent<TMP_Text>();
            _description = gameObject.transform.Find("Description").GetComponent<TMP_Text>();
            _cost = gameObject.transform.Find("Cost").GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            UpgradeListDisplay.OnUpgradeEquipped += Equip;
            UpgradeListDisplay.OnUpgradeUnequipped += Unequip;
            UpgradeListDisplay.OnUpgradeBought += OnUpgradeBoughtResult;
        }

        private void OnDisable()
        {
            UpgradeListDisplay.OnUpgradeEquipped -= Equip;
            UpgradeListDisplay.OnUpgradeUnequipped -= Unequip;
            UpgradeListDisplay.OnUpgradeBought -= OnUpgradeBoughtResult;
        }


        public void Initialize(UpgradeManager.Upgrade u)
        {
            Upgrade = u;
            Equipped = u.Equipped;
            _name.text = u.Name;
            _description.text = u.Description;
            //_cost.text = u.Unlocked ? "Unlocked!" : "$" + u.UnlockCost.ToString();
            _cost.text = u.Equipped ? "Equipped" : u.Unlocked ? "Not equipped" : "$" + u.UnlockCost.ToString();
            _image.color = u.Equipped ? Color.green : u.Unlocked ? _unlockedColor : Color.grey;
        }

        public void OnUpgradeBoughtResult(UpgradeManager.Upgrade u)
        {
            if (Upgrade != u) { return; }
            _image.color = _unlockedColor;
            _cost.text = "Not equipped";
        }

        public void Equip(UpgradeManager.Upgrade newUpgrade, UpgradeSlot slot, GameObject newButton)
        {
            // if this button is equipped in the active slot, but the new upgrade is not this upgrade, unslot this button
            if (EquippedSlot == slot && newUpgrade != Upgrade)
            {
                Equipped = false;
                EquippedSlot = null;
                _image.color = newUpgrade.Unlocked ? _unlockedColor : Color.grey;
                UpgradeManager.Instance.UnequipUpgrade(Upgrade);
                _cost.text = "Not equipped";
                return;
            }

            if (!Equipped && newUpgrade == Upgrade)
            {
                Equipped = true;
                EquippedSlot = slot;
                _image.color = Color.green;
                _cost.text = "Equipped";
                return;
            }
        }

        public void Unequip(UpgradeManager.Upgrade u, UpgradeSlot slot)
        {
            if (u != Upgrade) { return; }
            Equipped = false;
            EquippedSlot = null;
            _image.color = u.Unlocked ? _unlockedColor : Color.grey;
            _cost.text = "Not equipped";
        }

        public void OnSelect(BaseEventData eventData)
        {
            transform.localScale *= 1.1f;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            transform.localScale /= 1.1f;
        }
    }
}