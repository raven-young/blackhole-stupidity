using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BlackHole {
    public class UpgradeButton : MonoBehaviour
    {
        public UpgradeManager.Upgrade Upgrade;
        public bool Equipped;
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
            _cost.text = u.Unlocked ? "Unlocked!" : u.UnlockCost.ToString();
            _image.color = u.Equipped ? Color.green : u.Unlocked ? _unlockedColor : Color.grey;
        }

        public void OnUpgradeBoughtResult(UpgradeManager.Upgrade u)
        {
            if (Upgrade != u) { return; }
            _image.color = _unlockedColor;
            _cost.text = "Unlocked!";
        }

        public void Equip(UpgradeManager.Upgrade newUpgrade, UpgradeSlot slot)
        {
            
            // if the new upgrade is not this upgrade, unequip this upgrade
            if (slot.ActiveUpgrade == Upgrade && newUpgrade != Upgrade)
            {
                Debug.Log("unequipping upgrade button: " + slot + " " + Upgrade + " " + newUpgrade);
                Equipped = false;
                _image.color = newUpgrade.Unlocked ? _unlockedColor : Color.grey;
                UpgradeManager.Instance.UnequipUpgrade(Upgrade);
                return;
            }

            if (!Equipped && newUpgrade == Upgrade)
            {
                Debug.Log("equipping upgrade button: " + slot + " " + Upgrade + " " + newUpgrade);
                Equipped = true;
                _image.color = Color.green;
                return;
            }

            Debug.Log("reached end: " + slot + " " + Upgrade + " " + newUpgrade);
        }

        public void Unequip(UpgradeManager.Upgrade u, UpgradeSlot slot)
        {
            if (u != Upgrade) { return; }
            Equipped = false;
            Debug.Log("unequip: " + u + " " + u.Name + " " + u.Unlocked);
            _image.color = u.Unlocked ? _unlockedColor : Color.grey;
        }
    }
}